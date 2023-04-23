using System.Diagnostics;
using HomeRunTracker.Core.Actions.GameScores.Notifications;
using HomeRunTracker.Core.Actions.ScoringPlays.Notifications;
using HomeRunTracker.Core.Models;
using MediatR;

namespace HomeRunTracker.Backend.Grains;

// ReSharper disable once UnusedType.Global
public class GameGrain : Grain, IGameGrain
{
    private readonly IHttpService _httpService;
    private readonly LeverageIndexService _leverageIndexService;
    private readonly ILogger<GameGrain> _logger;
    private readonly IMediator _mediator;
    private readonly PitcherGameScoreService _pitcherGameScoreService;

    public GameGrain(ILogger<GameGrain> logger, IMediator mediator, IHttpService httpService,
        LeverageIndexService leverageIndexService, PitcherGameScoreService pitcherGameScoreService)
    {
        _logger = logger;
        _mediator = mediator;
        _httpService = httpService;
        _leverageIndexService = leverageIndexService;
        _pitcherGameScoreService = pitcherGameScoreService;
        
        GameId = (int) this.GetPrimaryKeyLong();
    }

    private MlbGameContent GameContent { get; set; } = null!;
    private MlbGameDetails GameDetails { get; set; } = null!;
    private HashSet<GameScoreRecord> GameScores { get; } = new();
    private HashSet<ScoringPlayRecord> ScoringPlays { get; } = new();
    private int GameId { get; }
    private DateTimeOffset GameStartTime { get; set; } = DateTimeOffset.MinValue;
    private bool IsInitialLoad { get; set; } = true;

    private List<string> ScoringPlayHashes => ScoringPlays.Select(x => x.Hash).ToList();

    public async Task<MlbGameDetails> GetGame()
    {
        _logger.LogInformation("Getting game details for game {GameId}", GameId.ToString());
        return await Task.FromResult(GameDetails);
    }

    public async Task Stop()
    {
        _logger.LogInformation("Stopping game grain {GameId}", GameId.ToString());
        if (IsInitialLoad) return;
        DeactivateOnIdle();
        await _mediator.Publish(new GameStoppedNotification(GameId));
    }

    public Task<List<ScoringPlayRecord>> GetScoringPlays()
    {
        _logger.LogInformation("Getting scoring plays for game {GameId}", GameId.ToString());
        return Task.FromResult(ScoringPlays.ToList());
    }

    public Task<List<GameScoreRecord>> GetGameScores()
    {
        _logger.LogInformation("Getting game scores for game {GameId}", GameId.ToString());
        return Task.FromResult(GameScores.ToList());
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initializing game grain {GameId}", GameId.ToString());

        await PollGame(new object());
        RegisterTimer(PollGame, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));

        await base.OnActivateAsync(cancellationToken);
    }

    private async Task PollGame(object _)
    {
        _logger.LogDebug("Polling game {GameId}", GameId.ToString());

        var fetchGameDetailsTask = _httpService.FetchGameDetails(GameId);
        var fetchGameContentTask = _httpService.FetchGameContent(GameId);

        await Task.WhenAll(fetchGameDetailsTask, fetchGameContentTask);

        if (fetchGameDetailsTask.Result.TryPickT2(out var error, out var rest))
            _logger.LogError("Failed to fetch game details from MLB API: {Error}", error.Value);

        if (rest.TryPickT1(out var failureStatusCode, out var gameDetails))
            _logger.LogError("Failed to fetch game details from MLB API; status code: {StatusCode}",
                failureStatusCode.ToString());

        if (fetchGameContentTask.Result.TryPickT2(out var error2, out var rest2))
            _logger.LogError("Failed to fetch game content from MLB API: {Error}", error2.Value);

        if (rest2.TryPickT1(out var failureStatusCode2, out var gameContent))
            _logger.LogError("Failed to fetch game content from MLB API; status code: {StatusCode}",
                failureStatusCode2.ToString());

        GameDetails = gameDetails;
        GameContent = gameContent;
        GameStartTime = GameDetails.GameData.GameDateTime.DateTimeOffset;

        if (!IsInitialLoad)
            switch (gameDetails.GameData.Status.Status)
            {
                case EMlbGameStatus.PreGame:
                    _logger.LogInformation("Game {GameId} is in pre-game", GameId.ToString());
                    await Task.Delay(TimeSpan.FromMinutes(15));
                    return;
                case EMlbGameStatus.Warmup:
                    _logger.LogInformation("Game {GameId} is warming up", GameId.ToString());
                    await Task.Delay(TimeSpan.FromMinutes(5));
                    return;
                case EMlbGameStatus.InProgress:
                    break;
                default:
                    _logger.LogInformation("Game {GameId} is no longer in progress", GameId.ToString());
                    await Stop();
                    return;
            }

        var scoringPlays = gameDetails.LiveData.Plays.AllPlays
            .Where(p => p.Result.Rbi > 0)
            .ToList();

        var tasks = scoringPlays
            .Where(play =>
            {
                if (IsInitialLoad) return true;

                var hash = ScoringPlayRecord.GetHash(play.Result.Description, GameId);
                return !ScoringPlayHashes.Contains(hash);
            })
            .Select(ValidateScoringPlay)
            .ToList();
        await Task.WhenAll(tasks);

        await CheckPitcherGameScores();

        IsInitialLoad = false;
    }

    private async Task CheckPitcherGameScores()
    {
        var gameScores = _pitcherGameScoreService.GetPitcherGameScores(GameDetails);
        foreach (var gameScore in gameScores)
        {
            var existingGameScore = GameScores.FirstOrDefault(x =>
                x.TeamId == gameScore.TeamId && x.PitcherId == gameScore.PitcherId);
            if (existingGameScore == null)
            {
                GameScores.Add(gameScore);
                if (!IsInitialLoad)
                    await _mediator.Publish(new GameScoreNotification(GameId, GameStartTime, gameScore));
                continue;
            }

            if (existingGameScore == gameScore) continue;

            GameScores.Remove(existingGameScore);
            GameScores.Add(gameScore);
            if (!IsInitialLoad)
                await _mediator.Publish(new GameScoreNotification(GameId, GameStartTime, gameScore));
        }
    }

    private async Task ValidateScoringPlay(MlbPlay play)
    {
        var descriptionHashString = ScoringPlayRecord.GetHash(play.Result.Description, GameId);

        var scoringPlayEvent = play.Events.SingleOrDefault(e => e.HitData is not null) ?? play.Events.Last();

        Debug.Assert(scoringPlayEvent is not null, nameof(scoringPlayEvent) + " is not null");

        var highlightUrl = (GameContent.HighlightsOverview?.Highlights?.Items ?? new List<HighlightItem>())
            .FirstOrDefault(item => item.Guid is not null && item.Guid == scoringPlayEvent.PlayId)
            ?.Playbacks.FirstOrDefault(p => p.PlaybackType is EPlaybackType.Mp4)
            ?.Url;

        if (ScoringPlayHashes.Contains(descriptionHashString))
        {
            _logger.LogDebug("Scoring play {Hash} has already been published", descriptionHashString);

            var existingScoringPlay = ScoringPlays.Single(p => p.Hash == descriptionHashString);
            if (highlightUrl is null || existingScoringPlay.HighlightUrl == highlightUrl) return;

            _logger.LogInformation("Scoring play {Hash} has a new highlight URL", descriptionHashString);
            existingScoringPlay.HighlightUrl = highlightUrl;
            if (!IsInitialLoad)
                await _mediator.Publish(new ScoringPlayUpdatedNotification(descriptionHashString, GameId,
                    GameStartTime, highlightUrl));

            return;
        }

        _logger.LogInformation("Game {GameId} has a new scoring play with hash {Hash}", GameId.ToString(),
            descriptionHashString);

        var (batterTeamId, pitcherTeamId, batterTeamName, pitcherTeamName, isTopInning) =
            new PlayTeams(play, GameDetails);

        var scoringPlayRecord = new ScoringPlayRecord
        {
            Hash = descriptionHashString,
            GameId = GameId,
            DateTimeOffset = play.DateTimeOffset,
            BatterId = play.PlayerMatchup.Batter.Id,
            BatterName = play.PlayerMatchup.Batter.FullName,
            Description = play.Result.Description,
            Rbi = play.Result.Rbi,
            LaunchSpeed = scoringPlayEvent.HitData?.LaunchSpeed ?? 0,
            LaunchAngle = scoringPlayEvent.HitData?.LaunchAngle ?? 0,
            TotalDistance = scoringPlayEvent.HitData?.TotalDistance ?? 0,
            Inning = play.About.Inning,
            IsTopInning = isTopInning,
            PitcherId = play.PlayerMatchup.Pitcher.Id,
            PitcherName = play.PlayerMatchup.Pitcher.FullName,
            HighlightUrl = highlightUrl,
            TeamId = batterTeamId,
            TeamName = batterTeamName,
            TeamNameAgainstId = pitcherTeamId,
            TeamNameAgainst = pitcherTeamName,
            LeverageIndex = _leverageIndexService.GetLeverageIndex(play),
            PlayResult = play.Result.Result
        };

        ScoringPlays.Add(scoringPlayRecord);
        if (!IsInitialLoad)
            await _mediator.Publish(new ScoringPlayNotification(GameId, GameStartTime, scoringPlayRecord));
    }
}
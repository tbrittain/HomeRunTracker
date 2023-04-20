﻿using System.Diagnostics;
using HomeRunTracker.Backend.Services;
using HomeRunTracker.Backend.Services.HttpService;
using HomeRunTracker.Common.Enums;
using HomeRunTracker.Common.Models.Content;
using HomeRunTracker.Common.Models.Details;
using HomeRunTracker.Common.Models.Internal;
using HomeRunTracker.Common.Models.Notifications;
using MediatR;

namespace HomeRunTracker.Backend.Grains;

// ReSharper disable once UnusedType.Global
public class GameGrain : Grain, IGameGrain
{
    private readonly HashSet<ScoringPlayRecord> _scoringPlays = new();
    private readonly HashSet<GameScoreRecord> _gameScores = new();
    private readonly IHttpService _httpService;
    private readonly ILogger<GameGrain> _logger;
    private readonly LeverageIndexService _leverageIndexService;
    private readonly GameScoreService _gameScoreService;
    private readonly IMediator _mediator;
    private MlbGameDetails _gameDetails = new();
    private MlbGameContent _gameContent = new();
    private DateTimeOffset _gameStartTime = DateTimeOffset.MinValue;
    private int _gameId;
    private bool _isInitialLoad = true;

    public GameGrain(ILogger<GameGrain> logger, IMediator mediator, IHttpService httpService,
        LeverageIndexService leverageIndexService, GameScoreService gameScoreService)
    {
        _logger = logger;
        _mediator = mediator;
        _httpService = httpService;
        _leverageIndexService = leverageIndexService;
        _gameScoreService = gameScoreService;
    }

    private List<string> ScoringPlayHashes => _scoringPlays.Select(x => x.Hash).ToList();

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _gameId = (int) this.GetPrimaryKeyLong();
        _logger.LogInformation("Initializing game grain {GameId}", _gameId.ToString());

        await PollGame(new object());
        RegisterTimer(PollGame, null, TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(5));

        await base.OnActivateAsync(cancellationToken);
    }

    private async Task PollGame(object _)
    {
        _logger.LogDebug("Polling game {GameId}", _gameId.ToString());

        var fetchGameDetailsTask = _httpService.FetchGameDetails(_gameId);
        var fetchGameContentTask = _httpService.FetchGameContent(_gameId);

        await Task.WhenAll(fetchGameDetailsTask, fetchGameContentTask);

        if (fetchGameDetailsTask.Result.TryPickT2(out var error, out var rest))
        {
            _logger.LogError("Failed to fetch game details from MLB API: {Error}", error.Value);
        }

        if (rest.TryPickT1(out var failureStatusCode, out var gameDetails))
        {
            _logger.LogError("Failed to fetch game details from MLB API; status code: {StatusCode}",
                failureStatusCode.ToString());
        }

        if (fetchGameContentTask.Result.TryPickT2(out var error2, out var rest2))
        {
            _logger.LogError("Failed to fetch game content from MLB API: {Error}", error2.Value);
        }

        if (rest2.TryPickT1(out var failureStatusCode2, out var gameContent))
        {
            _logger.LogError("Failed to fetch game content from MLB API; status code: {StatusCode}",
                failureStatusCode2.ToString());
        }

        _gameDetails = gameDetails;
        _gameContent = gameContent;
        _gameStartTime = _gameDetails.GameData.GameDateTime.DateTimeOffset;

        if (!_isInitialLoad)
        {
            switch (gameDetails.GameData.Status.Status)
            {
                case EMlbGameStatus.PreGame:
                    _logger.LogInformation("Game {GameId} is in pre-game", _gameId.ToString());
                    await Task.Delay(TimeSpan.FromMinutes(15));
                    return;
                case EMlbGameStatus.Warmup:
                    _logger.LogInformation("Game {GameId} is warming up", _gameId.ToString());
                    await Task.Delay(TimeSpan.FromMinutes(5));
                    return;
                case EMlbGameStatus.InProgress:
                    break;
                default:
                    _logger.LogInformation("Game {GameId} is no longer in progress", _gameId.ToString());
                    await Stop();
                    return;
            }
        }

        var scoringPlays = gameDetails.LiveData.Plays.AllPlays
            .Where(p => p.Result.Rbi > 0)
            .ToList();
        
        var tasks = scoringPlays
            .Where(play =>
            {
                if (_isInitialLoad) return true;
        
                var hash = ScoringPlayRecord.GetHash(play.Result.Description, _gameId);
                return !ScoringPlayHashes.Contains(hash);
            })
            .Select(ValidateScoringPlay)
            .ToList();
        await Task.WhenAll(tasks);

        var gameScores = _gameScoreService.GetGameScores(_gameDetails);
        foreach (var gameScore in gameScores)
        {
            var existingGameScore = _gameScores.FirstOrDefault(x =>
                x.TeamId == gameScore.TeamId && x.PitcherId == gameScore.PitcherId);
            if (existingGameScore == null)
            {
                _gameScores.Add(gameScore);
                if (!_isInitialLoad)
                    await _mediator.Publish(new GameScoreNotification(_gameId, _gameStartTime, gameScore));
                continue;
            }

            if (existingGameScore == gameScore) continue;

            _gameScores.Remove(existingGameScore);
            _gameScores.Add(gameScore);
            if (!_isInitialLoad)
                await _mediator.Publish(new GameScoreNotification(_gameId, _gameStartTime, gameScore));
        }

        _isInitialLoad = false;
    }

    public async Task<MlbGameDetails> GetGame()
    {
        _logger.LogInformation("Getting game details for game {GameId}", _gameId.ToString());
        return await Task.FromResult(_gameDetails);
    }

    public async Task Stop()
    {
        _logger.LogInformation("Stopping game grain {GameId}", _gameId.ToString());
        if (_isInitialLoad) return;
        DeactivateOnIdle();
        await _mediator.Publish(new GameStoppedNotification(_gameId));
    }

    public Task<List<ScoringPlayRecord>> GetScoringPlays()
    {
        _logger.LogInformation("Getting scoring plays for game {GameId}", _gameId.ToString());
        return Task.FromResult(_scoringPlays.ToList());
    }

    public Task<List<GameScoreRecord>> GetGameScores()
    {
        _logger.LogInformation("Getting game scores for game {GameId}", _gameId.ToString());
        return Task.FromResult(_gameScores.ToList());
    }

    private async Task ValidateScoringPlay(MlbPlay play)
    {
        var descriptionHashString = ScoringPlayRecord.GetHash(play.Result.Description, _gameId);

        var scoringPlayEvent = play.Events.SingleOrDefault(e => e.HitData is not null) ?? play.Events.Last();

        Debug.Assert(scoringPlayEvent is not null, nameof(scoringPlayEvent) + " is not null");

        var highlightUrl = (_gameContent.HighlightsOverview?.Highlights?.Items ?? new List<HighlightItem>())
            .FirstOrDefault(item => item.Guid is not null && item.Guid == scoringPlayEvent.PlayId)
            ?.Playbacks.FirstOrDefault(p => p.PlaybackType is EPlaybackType.Mp4)
            ?.Url;

        if (ScoringPlayHashes.Contains(descriptionHashString))
        {
            _logger.LogDebug("Scoring play {Hash} has already been published", descriptionHashString);

            var existingScoringPlay = _scoringPlays.Single(p => p.Hash == descriptionHashString);
            if (highlightUrl is null || existingScoringPlay.HighlightUrl == highlightUrl) return;

            _logger.LogInformation("Scoring play {Hash} has a new highlight URL", descriptionHashString);
            existingScoringPlay.HighlightUrl = highlightUrl;
            if (!_isInitialLoad)
                await _mediator.Publish(new ScoringPlayUpdatedNotification(descriptionHashString, _gameId,
                    _gameStartTime, highlightUrl));

            return;
        }

        _logger.LogInformation("Game {GameId} has a new scoring play with hash {Hash}", _gameId.ToString(),
            descriptionHashString);

        var (batterTeamId, pitcherTeamId, batterTeamName, pitcherTeamName, isTopInning) =
            new PlayTeams(play, _gameDetails);

        var scoringPlayRecord = new ScoringPlayRecord
        {
            Hash = descriptionHashString,
            GameId = _gameId,
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

        _scoringPlays.Add(scoringPlayRecord);
        if (!_isInitialLoad)
        {
            await _mediator.Publish(new ScoringPlayNotification(_gameId, _gameStartTime, scoringPlayRecord));
        }
    }
}
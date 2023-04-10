using System.Diagnostics;
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
    private readonly HashSet<HomeRunRecord> _homeRuns = new();
    private readonly IHttpService _httpService;
    private readonly ILogger<GameGrain> _logger;
    private readonly IMediator _mediator;
    private MlbGameDetails _gameDetails = new();
    private MlbGameContent _gameContent = new();
    private int _gameId;
    private bool _isInitialLoad = true;

    public GameGrain(ILogger<GameGrain> logger, IMediator mediator, IHttpService httpService)
    {
        _logger = logger;
        _mediator = mediator;
        _httpService = httpService;
    }

    private List<string> HomeRunHashes => _homeRuns.Select(x => x.Hash).ToList();

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _gameId = (int) this.GetPrimaryKeyLong();
        _logger.LogInformation("Initializing game grain {GameId}", _gameId.ToString());

        await PollGame(new object());
        RegisterTimer(PollGame, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));

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

        var homeRuns = gameDetails.LiveData.Plays.AllPlays
            .Where(p => p.Result.Result is EPlayResult.HomeRun)
            .ToList();

        var tasks = homeRuns
            .Where(homeRun =>
            {
                if (_isInitialLoad) return true;
                
                var hash = HomeRunRecord.GetHash(homeRun.Result.Description, _gameId);
                return !HomeRunHashes.Contains(hash);
            })
            .Select(ValidateHomeRun)
            .ToList();
        await Task.WhenAll(tasks);
        
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
        DeactivateOnIdle();
        await _mediator.Publish(new GameStoppedNotification(_gameId));
    }

    public Task<List<HomeRunRecord>> GetHomeRuns()
    {
        _logger.LogInformation("Getting home runs for game {GameId}", _gameId.ToString());
        return Task.FromResult(_homeRuns.ToList());
    }

    private async Task ValidateHomeRun(MlbPlay play)
    {
        var descriptionHashString = HomeRunRecord.GetHash(play.Result.Description, _gameId);

        var homeRunEvent = play.Events.Single(e => e.HitData is not null);
        Debug.Assert(homeRunEvent is not null, nameof(homeRunEvent) + " is not null");

        var highlightUrl = _gameContent.HighlightsOverview.Highlights.Items
            .FirstOrDefault(item => item.Guid is not null && item.Guid == homeRunEvent.PlayId)
            ?.Playbacks.FirstOrDefault(p => p.PlaybackType is EPlaybackType.Mp4)
            ?.Url;

        if (HomeRunHashes.Contains(descriptionHashString))
        {
            _logger.LogDebug("Home run {Hash} has already been published", descriptionHashString);

            var existingHomeRun = _homeRuns.Single(hr => hr.Hash == descriptionHashString);
            if (highlightUrl is null || existingHomeRun.HighlightUrl == highlightUrl) return;

            _logger.LogInformation("Home run {Hash} has a new highlight URL", descriptionHashString);
            existingHomeRun.HighlightUrl = highlightUrl;
            if (!_isInitialLoad)
            {
                await _mediator.Publish(new HomeRunUpdatedNotification(descriptionHashString, _gameId, highlightUrl));
            }

            return;
        }

        _logger.LogInformation("Game {GameId} has a new home run with hash {Hash}", _gameId.ToString(),
            descriptionHashString);

        var homeRunRecord = new HomeRunRecord
        {
            Hash = descriptionHashString,
            GameId = _gameId,
            DateTime = play.DateTime,
            BatterId = play.PlayerMatchup.Batter.Id,
            BatterName = play.PlayerMatchup.Batter.FullName,
            Description = play.Result.Description,
            Rbi = play.Result.Rbi,
            LaunchSpeed = homeRunEvent.HitData!.LaunchSpeed,
            LaunchAngle = homeRunEvent.HitData!.LaunchAngle,
            TotalDistance = homeRunEvent.HitData!.TotalDistance,
            Inning = play.About.Inning,
            IsTopInning = play.About.IsTopInning,
            PitcherId = play.PlayerMatchup.Pitcher.Id,
            PitcherName = play.PlayerMatchup.Pitcher.FullName,
            HighlightUrl = highlightUrl
        };

        _homeRuns.Add(homeRunRecord);
        if (!_isInitialLoad)
        {
            await _mediator.Publish(new HomeRunNotification(_gameId, homeRunRecord));
        }
    }
}
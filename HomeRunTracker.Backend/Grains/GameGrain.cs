using System.Diagnostics;
using HomeRunTracker.Backend.Services.HttpService;
using HomeRunTracker.Common.Models.Details;
using HomeRunTracker.Common.Models.Internal;
using HomeRunTracker.Common.Models.Notifications;
using HomeRunTracker.Common.Models.Summary;
using MediatR;
using Newtonsoft.Json;

namespace HomeRunTracker.Backend.Grains;

public class GameGrain : Grain, IGameGrain
{
    private readonly List<string> _homeRunHashes = new();
    private readonly HashSet<MlbPlay> _homeRuns = new();
    private readonly IHttpService _httpService;
    private readonly ILogger<GameGrain> _logger;
    private readonly IMediator _mediator;
    private string _gameContentLink = string.Empty; // TODO: use this, check #2
    private MlbGameDetails _gameDetails = new();
    private int _gameId;
    private bool _isInitialLoad = true;

    public GameGrain(ILogger<GameGrain> logger, IMediator mediator, IHttpService httpService)
    {
        _logger = logger;
        _mediator = mediator;
        _httpService = httpService;
    }

    public async Task<int> InitializeAsync(MlbGameSummary game)
    {
        _gameId = game.Id;
        _gameContentLink = game.Content.Link;

        _logger.LogInformation("Initializing game grain {GameId}", _gameId.ToString());
        RegisterTimer(async _ =>
        {
            _logger.LogDebug("Polling game {GameId}", _gameId.ToString());

            var gameDetails = await _httpService.FetchGameDetailsAsync(_gameId);

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
            }

            var homeRuns = gameDetails.LiveData.Plays.AllPlays
                .Where(p => p.Result.Result is EPlayResult.HomeRun)
                .ToList();

            var tasks = homeRuns
                .Where(homeRun => !_homeRuns.Contains(homeRun))
                .Select(ValidateHomeRun)
                .ToList();
            await Task.WhenAll(tasks);

            if (gameDetails.GameData.Status.Status is not EMlbGameStatus.InProgress)
            {
                _logger.LogInformation("Game {GameId} is no longer in progress", _gameId.ToString());
                await StopAsync();
                return;
            }

            _gameDetails = gameDetails;
            _isInitialLoad = false;
        }, null, TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(1));

        await base.OnActivateAsync(default);
        return _gameId;
    }

    public async Task<MlbGameDetails> GetGameAsync()
    {
        return await Task.FromResult(_gameDetails);
    }

    public async Task StopAsync()
    {
        _logger.LogInformation("Stopping game grain {GameId}", _gameId.ToString());
        DeactivateOnIdle();
        await _mediator.Publish(new GameStoppedNotification(_gameId));
    }

    private async Task ValidateHomeRun(MlbPlay play)
    {
        var descriptionHashString = HomeRunRecord.GetHash(play.Result.Description, _gameId);
        if (_isInitialLoad)
        {
            _homeRunHashes.Add(descriptionHashString);
            return;
        }

        var homeRunEvent = play.Events.Single(e => e.HitData is not null).HitData;
        Debug.Assert(homeRunEvent != null, nameof(homeRunEvent) + " != null");

        if (_homeRunHashes.Contains(descriptionHashString))
        {
            _logger.LogDebug("Home run {Hash} has already been published", descriptionHashString);
            return;
        }

        _logger.LogInformation("Game {GameId} has a new home run with hash {Hash}", _gameId.ToString(),
            descriptionHashString);
        _homeRuns.Add(play);

        var homeRunRecord = new HomeRunRecord
        {
            Hash = descriptionHashString,
            GameId = _gameId,
            DateTime = play.DateTime,
            BatterId = play.PlayerMatchup.Batter.Id,
            BatterName = play.PlayerMatchup.Batter.FullName,
            Description = play.Result.Description,
            Rbi = play.Result.Rbi,
            LaunchSpeed = homeRunEvent.LaunchSpeed,
            LaunchAngle = homeRunEvent.LaunchAngle,
            TotalDistance = homeRunEvent.TotalDistance,
            Inning = play.About.Inning,
            IsTopInning = play.About.IsTopInning,
            PitcherId = play.PlayerMatchup.Pitcher.Id,
            PitcherName = play.PlayerMatchup.Pitcher.FullName
        };

        await _mediator.Publish(new HomeRunNotification(_gameId, homeRunRecord));
        _homeRunHashes.Add(descriptionHashString);
    }
}
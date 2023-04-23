using HomeRunTracker.Backend.Hubs;
using HomeRunTracker.Core.Actions.GameScores.Notifications;
using HomeRunTracker.Core.Actions.ScoringPlays.Notifications;
using HomeRunTracker.Core.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace HomeRunTracker.Backend.Grains;

// ReSharper disable once UnusedType.Global
public class GameListGrain : Grain, IGameListGrain
{
    private readonly IClusterClient _clusterClient;
    private readonly IHttpService _httpService;
    private readonly IHubContext<ScoringPlayHub> _hubContext;
    private readonly ILogger<GameListGrain> _logger;

    public GameListGrain(IClusterClient clusterClient, ILogger<GameListGrain> logger,
        IHubContext<ScoringPlayHub> hubContext, IHttpService httpService)
    {
        _clusterClient = clusterClient;
        _logger = logger;
        _hubContext = hubContext;
        _httpService = httpService;
    }

    public async Task<List<ScoringPlayRecord>> GetScoringPlays(DateTime dateTime)
    {
        _logger.LogInformation("Getting all scoring plays for {Date}", dateTime.ToString("yyyy-MM-dd"));

        var gameGrains = await GetGameGrains(dateTime);
        if (!gameGrains.Any()) return new List<ScoringPlayRecord>();

        var tasks = gameGrains
            .Select(grain => grain.GetScoringPlays())
            .ToList();

        await Task.WhenAll(tasks);

        var allScoringPlays = tasks
            .SelectMany(x => x.Result)
            .ToList();

        _logger.LogInformation("Returning {NumScoringPlays} scoring plays", allScoringPlays.Count.ToString());
        return allScoringPlays;
    }

    public async Task<List<GameScoreRecord>> GetGameScores(DateTime dateTime)
    {
        _logger.LogInformation("Getting all game scores for {Date}", dateTime.ToString("yyyy-MM-dd"));

        var gameGrains = await GetGameGrains(dateTime);
        if (!gameGrains.Any()) return new List<GameScoreRecord>();

        var tasks = gameGrains
            .Select(grain => grain.GetGameScores())
            .ToList();

        await Task.WhenAll(tasks);

        var allGameScores = tasks
            .SelectMany(x => x.Result)
            .ToList();

        _logger.LogInformation("Returning {NumGameScores} game scores", allGameScores.Count.ToString());
        return allGameScores;
    }

    public async Task PublishScoringPlay(ScoringPlayNotification notification)
    {
        _logger.LogInformation("Publishing scoring play {Hash} for game {GameId}", notification.ScoringPlay.Hash,
            notification.GameId.ToString());

        await _hubContext.Clients.All.SendAsync("ReceiveScoringPlay", JsonConvert.SerializeObject(notification));

        _logger.LogInformation("Finished publishing scoring play {Hash} for game {GameId}",
            notification.ScoringPlay.Hash, notification.GameId.ToString());
    }

    public async Task PublishScoringPlayUpdated(ScoringPlayUpdatedNotification notification)
    {
        _logger.LogInformation("Publishing scoring play modified {Hash} for game {GameId}", notification.HomeRunHash,
            notification.GameId.ToString());

        await _hubContext.Clients.All.SendAsync("UpdateScoringPlay", JsonConvert.SerializeObject(notification));

        _logger.LogInformation("Finished publishing scoring play modified {Hash} for game {GameId}",
            notification.HomeRunHash, notification.GameId.ToString());
    }

    public async Task PublishGameScore(GameScoreNotification notification)
    {
        _logger.LogInformation("Publishing game score for game {GameId}", notification.GameId.ToString());

        await _hubContext.Clients.All.SendAsync("ReceiveGameScore", JsonConvert.SerializeObject(notification));

        _logger.LogInformation("Finished publishing game score for game {GameId}", notification.GameId.ToString());
    }

    private async Task<List<IGameGrain>> GetGameGrains(DateTime dateTime)
    {
        var fetchGamesResponse = await _httpService.FetchGames(dateTime);

        if (fetchGamesResponse.TryPickT2(out var error, out var rest))
            _logger.LogError("Failed to fetch games from MLB API: {Error}", error.Value);

        if (rest.TryPickT1(out var failureStatusCode, out var gameSchedule))
            _logger.LogError("Failed to fetch games from MLB API; status code: {StatusCode}",
                failureStatusCode.ToString());

        if (gameSchedule.TotalGames == 0) return new List<IGameGrain>();

        var gameIds = gameSchedule.Dates
            .SelectMany(x => x.Games
                .Select(y => y.Id))
            .ToList();

        var gameGrains = gameIds
            .Select(id => _clusterClient.GetGrain<IGameGrain>(id))
            .ToList();
        return gameGrains;
    }
}
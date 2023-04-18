using HomeRunTracker.Backend.Hubs;
using HomeRunTracker.Backend.Services;
using HomeRunTracker.Backend.Services.HttpService;
using HomeRunTracker.Common.Models.Internal;
using HomeRunTracker.Common.Models.Notifications;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace HomeRunTracker.Backend.Grains;

// ReSharper disable once UnusedType.Global
public class GameListGrain : Grain, IGameListGrain
{
    private readonly IClusterClient _clusterClient;
    private readonly ILogger<GameListGrain> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<ScoringPlayHub> _hubContext;
    private readonly IHttpService _httpService;

    public GameListGrain(IClusterClient clusterClient, ILogger<GameListGrain> logger, IServiceProvider serviceProvider,
        IHubContext<ScoringPlayHub> hubContext, IHttpService httpService)
    {
        _clusterClient = clusterClient;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
        _httpService = httpService;
    }

    public async Task<List<ScoringPlayRecord>> GetScoringPlays(DateTime dateTime)
    {
        _logger.LogInformation("Getting all scoring plays for {Date}", dateTime.ToString("yyyy-MM-dd"));
        var pollingService = _serviceProvider.GetService<MlbCurrentDayGamePollingService>();
        if (pollingService is null)
            throw new InvalidOperationException("MlbApiPollingService not found");

        var fetchGamesResponse = await _httpService.FetchGames(dateTime);

        if (fetchGamesResponse.TryPickT2(out var error, out var rest))
        {
            _logger.LogError("Failed to fetch games from MLB API: {Error}", error.Value);
        }

        if (rest.TryPickT1(out var failureStatusCode, out var gameSchedule))
        {
            _logger.LogError("Failed to fetch games from MLB API; status code: {StatusCode}",
                failureStatusCode.ToString());
        }

        if (gameSchedule.TotalGames == 0)
            return new List<ScoringPlayRecord>();

        var gameIds = gameSchedule.Dates
            .SelectMany(x => x.Games
                .Select(y => y.Id))
            .ToList();

        var gameGrains = gameIds
            .Select(id => _clusterClient.GetGrain<IGameGrain>(id))
            .ToList();

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

    public async Task PublishScoringPlay(ScoringPlayNotification notification)
    {
        _logger.LogInformation("Publishing scoring play {Hash} for game {GameId}", notification.ScoringPlay.Hash,
            notification.GameId.ToString());

        await _hubContext.Clients.All.SendAsync("ReceiveHomeRun", JsonConvert.SerializeObject(notification));

        _logger.LogInformation("Finished publishing scoring play {Hash} for game {GameId}", notification.ScoringPlay.Hash,
            notification.GameId.ToString());
    }

    public async Task PublishScoringPlayUpdated(ScoringPlayUpdatedNotification notification)
    {
        _logger.LogInformation("Publishing scoring play modified {Hash} for game {GameId}", notification.HomeRunHash,
            notification.GameId.ToString());
        
        await _hubContext.Clients.All.SendAsync("UpdateHomeRun", JsonConvert.SerializeObject(notification));
        
        _logger.LogInformation("Finished publishing scoring play modified {Hash} for game {GameId}", notification.HomeRunHash,
            notification.GameId.ToString());
    }
}
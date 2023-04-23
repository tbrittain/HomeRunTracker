using HomeRunTracker.Backend.Grains;
using HomeRunTracker.Core.Interfaces;

namespace HomeRunTracker.Backend.Services;

public class MlbCurrentDayGamePollingService : BackgroundService
{
    private readonly IMlbApiService _mlbApiService;
    private readonly IGrainFactory _grainFactory;
    private readonly ILogger<MlbCurrentDayGamePollingService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromHours(4);
    private readonly List<int> _trackedCurrentDayGameIds = new(15);

    public MlbCurrentDayGamePollingService(IGrainFactory grainFactory, ILogger<MlbCurrentDayGamePollingService> logger,
        IMlbApiService mlbApiService)
    {
        _grainFactory = grainFactory;
        _logger = logger;
        _mlbApiService = mlbApiService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting MLB API polling service");
        while (!stoppingToken.IsCancellationRequested)
        {
            var fetchGamesResponse = await _mlbApiService.FetchGames(DateTime.Now);

            if (fetchGamesResponse.TryPickT2(out var error, out var rest))
            {
                _logger.LogError("Failed to fetch games from MLB API: {Error}", error.Value);
            }

            if (rest.TryPickT1(out var failureStatusCode, out var schedule))
            {
                _logger.LogError("Failed to fetch games from MLB API; status code: {StatusCode}",
                    failureStatusCode.ToString());
            }
            
            // TODO: MAY NEED TO MAP SCHEDULE TO A NEW MODEL THAT HAS GenerateSerializerAttribute

            if (schedule.TotalGames == 0)
            {
                _logger.LogInformation("No games scheduled for today");
                await Task.Delay(_pollingInterval, stoppingToken);
                continue;
            }

            var games = schedule.Games
                .Where(g => !_trackedCurrentDayGameIds.Contains(g.Id))
                .ToList();

            var trackedGameIds = await FanOutGameGrains(games);
            _trackedCurrentDayGameIds.AddRange(trackedGameIds);

            await Task.Delay(_pollingInterval, stoppingToken);
        }

        foreach (var trackedGameId in _trackedCurrentDayGameIds)
        {
            var gameGrain = _grainFactory.GetGrain<IGameGrain>(trackedGameId);
            await gameGrain.Stop();
        }

        _logger.LogInformation("Stopping MLB API polling service");
    }

    private async Task<List<int>> FanOutGameGrains(List<MlbGameSummary> games)
    {
        _logger.LogInformation("Fanning out {Count} game grains", games.Count.ToString());
        List<Task<MlbGameDetails>> initializedGameTasks = new();
        foreach (var game in games)
        {
            var grain = _grainFactory.GetGrain<IGameGrain>(game.Id);
            var task = grain.GetGame();
            initializedGameTasks.Add(task);
        }

        await Task.WhenAll(initializedGameTasks);

        return initializedGameTasks.Select(t => t.Result.Id).ToList();
    }

    public void UntrackGame(int gameId)
    {
        _trackedCurrentDayGameIds.Remove(gameId);
    }
}
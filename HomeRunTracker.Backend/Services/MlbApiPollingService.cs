using HomeRunTracker.Backend.Grains;
using HomeRunTracker.Backend.Services.HttpService;
using HomeRunTracker.Common.Models.Summary;

namespace HomeRunTracker.Backend.Services;

public class MlbApiPollingService : BackgroundService
{
    private readonly IGrainFactory _grainFactory;
    private readonly IHttpService _httpService;
    private readonly ILogger<MlbApiPollingService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromHours(4);
    internal readonly List<int> TrackedCurrentDayGameIds = new();

    public MlbApiPollingService(IGrainFactory grainFactory, ILogger<MlbApiPollingService> logger, IHttpService httpService)
    {
        _grainFactory = grainFactory;
        _logger = logger;
        _httpService = httpService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting MLB API polling service");
        while (!stoppingToken.IsCancellationRequested)
        {
            var schedule = await _httpService.FetchGamesAsync(DateTime.Now);

            if (schedule.TotalGames == 0)
            {
                _logger.LogInformation("No games scheduled for today");
                await Task.Delay(_pollingInterval, stoppingToken);
                continue;
            }

            var games = schedule.Dates[0].Games
                .Where(g => !TrackedCurrentDayGameIds.Contains(g.Id))
                .ToList();

            var trackedGameIds = await FanOutGameGrainsAsync(games);
            TrackedCurrentDayGameIds.AddRange(trackedGameIds);

            await Task.Delay(_pollingInterval, stoppingToken);
        }

        foreach (var trackedGameId in TrackedCurrentDayGameIds)
        {
            var gameGrain = _grainFactory.GetGrain<IGameGrain>(trackedGameId);
            await gameGrain.StopAsync();
        }

        _logger.LogInformation("Stopping MLB API polling service");
    }

    private async Task<List<int>> FanOutGameGrainsAsync(List<MlbGameSummary> games)
    {
        _logger.LogInformation("Fanning out {Count} game grains", games.Count.ToString());
        var tasks = new List<Task<int>>();
        foreach (var game in games)
        {
            var gameGrain = _grainFactory.GetGrain<IGameGrain>(game.Id);
            tasks.Add(gameGrain.InitializeAsync(game));
        }

        await Task.WhenAll(tasks);
        return tasks.Select(t => t.Result).ToList();
    }

    public void RemoveGame(int gameId)
    {
        TrackedCurrentDayGameIds.Remove(gameId);
    }
}
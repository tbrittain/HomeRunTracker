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
    private readonly List<int> _trackedCurrentDayGameIds = new();

    public MlbApiPollingService(IGrainFactory grainFactory, ILogger<MlbApiPollingService> logger,
        IHttpService httpService)
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
                .Where(g => !_trackedCurrentDayGameIds.Contains(g.Id))
                .ToList();

            var trackedGameIds = await FanOutGameGrainsAsync(games);
            _trackedCurrentDayGameIds.AddRange(trackedGameIds);

            await Task.Delay(_pollingInterval, stoppingToken);
        }

        foreach (var trackedGameId in _trackedCurrentDayGameIds)
        {
            var gameGrain = _grainFactory.GetGrain<IGameGrain>(trackedGameId);
            await gameGrain.StopAsync();
        }

        _logger.LogInformation("Stopping MLB API polling service");
    }

    private Task<List<int>> FanOutGameGrainsAsync(List<MlbGameSummary> games)
    {
        _logger.LogInformation("Fanning out {Count} game grains", games.Count.ToString());
        foreach (var game in games)
        {
            _ = _grainFactory.GetGrain<IGameGrain>(game.Id);
        }

        return Task.FromResult(games.Select(g => g.Id).ToList());
    }

    public void RemoveGame(int gameId)
    {
        _trackedCurrentDayGameIds.Remove(gameId);
    }
}
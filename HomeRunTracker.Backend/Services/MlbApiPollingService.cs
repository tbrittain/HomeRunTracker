using HomeRunTracker.Backend.Grains;
using HomeRunTracker.Common.Models.Summary;
using Newtonsoft.Json;

namespace HomeRunTracker.Backend.Services;

public class MlbApiPollingService : BackgroundService
{
    private readonly IGrainFactory _grainFactory;
    private readonly HttpClient _httpClient;
    private readonly ILogger<MlbApiPollingService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromHours(4);
    internal readonly List<int> TrackedGameIds = new();

    public MlbApiPollingService(HttpClient httpClient, IGrainFactory grainFactory, ILogger<MlbApiPollingService> logger)
    {
        _httpClient = httpClient;
        _grainFactory = grainFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting MLB API polling service");
        while (!stoppingToken.IsCancellationRequested)
        {
            var schedule = await FetchGamesAsync();

            if (schedule.TotalGames == 0)
            {
                _logger.LogInformation("No games scheduled for today");
                await Task.Delay(_pollingInterval, stoppingToken);
                continue;
            }

            var newGames = schedule.Dates[0].Games
                .Where(g => !TrackedGameIds.Contains(g.Id))
                .Where(g => g.GameStatus.Status is EMlbGameStatus.InProgress or EMlbGameStatus.PreGame)
                .ToList();

            var trackedGameIds = await FanOutGameGrainsAsync(newGames);
            TrackedGameIds.AddRange(trackedGameIds);

            await Task.Delay(_pollingInterval, stoppingToken);
        }

        foreach (var trackedGameId in TrackedGameIds)
        {
            var gameGrain = _grainFactory.GetGrain<IGameGrain>(trackedGameId);
            await gameGrain.StopAsync();
        }

        _logger.LogInformation("Stopping MLB API polling service");
    }

    private async Task<MlbSchedule> FetchGamesAsync()
    {
        var today = DateTime.Now.ToString("yyyy-MM-dd");
        var url = $"https://statsapi.mlb.com/api/v1/schedule/games/?sportId=1&startDate={today}&endDate={today}";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch games from MLB API; status code: {StatusCode}", response.StatusCode);
            throw new Exception("Failed to fetch games from MLB API; status code: " + response.StatusCode);
        }

        var content = await response.Content.ReadAsStringAsync();
        var schedule = JsonConvert.DeserializeObject<MlbSchedule>(content);

        if (schedule is null)
        {
            _logger.LogError("Failed to deserialize MLB API response");
            throw new Exception("Failed to deserialize MLB API response");
        }

        return schedule;
    }

    private async Task<List<int>> FanOutGameGrainsAsync(List<MlbGameSummary> games)
    {
        _logger.LogInformation("Fanning out {Count} game grains", games.Count.ToString());
        var tasks = new List<Task<int>>();
        foreach (var game in games)
        {
            var gameGrain = _grainFactory.GetGrain<IGameGrain>(game.Id);
            tasks.Add(gameGrain.InitializeAsync(game));
            gameGrain.OnGameStopped += OnGameStopped;
        }

        await Task.WhenAll(tasks);
        return tasks.Select(t => t.Result).ToList();
    }

    private void OnGameStopped(object? sender, GameGrain.GameStoppedEventArgs e)
    {
        TrackedGameIds.Remove(e.GameId);
    }
}
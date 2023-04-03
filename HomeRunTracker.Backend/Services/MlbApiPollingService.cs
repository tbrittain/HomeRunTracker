using HomeRunTracker.Backend.Grains;
using HomeRunTracker.Common;
using Newtonsoft.Json;

namespace HomeRunTracker.Backend.Services;

public class MlbApiPollingService : BackgroundService
{
    private readonly HttpClient _httpClient;
    private readonly IGrainFactory _grainFactory;
    private readonly TimeSpan _pollingInterval;
    private readonly ILogger<MlbApiPollingService> _logger;
    private readonly List<int> _trackedGameIds = new();

    public MlbApiPollingService(HttpClient httpClient, IGrainFactory grainFactory, ILogger<MlbApiPollingService> logger)
    {
        _httpClient = httpClient;
        _grainFactory = grainFactory;
        _logger = logger;

        _pollingInterval = TimeSpan.FromDays(1);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting MLB API polling service");
        while (!stoppingToken.IsCancellationRequested)
        {
            var games = await FetchGamesAsync();

            var newGameGrains = games
                .Where(g => !_trackedGameIds.Contains(g.Id))
                .ToList();
            
            await FanOutGameGrainsAsync(newGameGrains);

            await Task.Delay(_pollingInterval, stoppingToken);
        }
        
        foreach (var trackedGameId in _trackedGameIds)
        {
            var gameGrain = _grainFactory.GetGrain<IGameGrain>(trackedGameId);
            await gameGrain.StopAsync();
        }

        _logger.LogInformation("Stopping MLB API polling service");
    }

    private async Task<List<Post>> FetchGamesAsync()
    {
        var response = await _httpClient.GetAsync("https://jsonplaceholder.typicode.com/posts");
        var content = await response.Content.ReadAsStringAsync();
        var posts = JsonConvert.DeserializeObject<List<Post>>(content)
            .Take(10)
            .ToList();
        return posts ?? new List<Post>();
    }

    private async Task FanOutGameGrainsAsync(List<Post> games)
    {
        _logger.LogInformation("Fanning out {Count} game grains", games.Count.ToString());
        var gameIds = new List<int>();
        foreach (var game in games)
        {
            var gameGrain = _grainFactory.GetGrain<IGameGrain>(game.Id);
            await gameGrain.InitializeAsync(game);
            gameIds.Add(game.Id);
        }
        
        _trackedGameIds.AddRange(gameIds);
    }
}
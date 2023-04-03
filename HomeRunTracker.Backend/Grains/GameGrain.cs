using HomeRunTracker.Common;
using Newtonsoft.Json;

namespace HomeRunTracker.Backend.Grains;

public class GameGrain : Grain, IGameGrain
{
    private Post _game = null!;
    private IDisposable _timer = null!;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GameGrain> _logger;

    public GameGrain(HttpClient httpClient, ILogger<GameGrain> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task InitializeAsync(Post game)
    {
        _game = game;

        _logger.LogInformation("Initializing game grain {GameId}", game.Id.ToString());
        _timer = RegisterTimer(async _ =>
        {
            _logger.LogDebug("Polling game {GameId}", _game.Id.ToString());
            
            var updatedGame = await FetchGameDataAsync(_game);

            if (updatedGame.Id > 4)
            {
                _logger.LogInformation("Game {GameId} is over", _game.Id.ToString());
                await StopAsync();
            }
            else
            {
                _game = updatedGame;
            }
        }, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));

        await base.OnActivateAsync(default);
    }

    public async Task<Post> GetGameAsync()
    {
        return await Task.FromResult(_game);
    }

    public async Task StopAsync()
    {
        _logger.LogInformation("Stopping game grain {GameId}", _game.Id.ToString());
        DeactivateOnIdle();
        await Task.CompletedTask;
    }

    private async Task<Post> FetchGameDataAsync(Post game)
    {
        _logger.LogDebug("Fetching game data for game {GameId}", game.Id.ToString());
        
        var response = await _httpClient.GetAsync($"https://jsonplaceholder.typicode.com/posts/{game.Id}");
        var content = await response.Content.ReadAsStringAsync();
        var updatedGame = JsonConvert.DeserializeObject<Post>(content);
        return updatedGame;
    }
}
using HomeRunTracker.Common.Models.Details;
using HomeRunTracker.Common.Models.Summary;
using Newtonsoft.Json;

namespace HomeRunTracker.Backend.Services.HttpService;

public class HttpService : IHttpService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HttpService> _logger;

    public HttpService(IHttpClientFactory httpClientFactory, ILogger<HttpService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<MlbSchedule> FetchGames(DateTime dateTime)
    {
        var formattedDate = dateTime.ToString("yyyy-MM-dd");
        var url =
            $"https://statsapi.mlb.com/api/v1/schedule/games/?sportId=1&startDate={formattedDate}&endDate={formattedDate}";

        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.GetAsync(url);
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

    public async Task<MlbGameDetails> FetchGameDetails(int gameId)
    {
        _logger.LogDebug("Fetching game data for game {GameId}", gameId.ToString());
        var url = $"https://statsapi.mlb.com/api/v1.1/game/{gameId}/feed/live";

        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch game data for game {GameId}", gameId.ToString());
            throw new Exception($"Failed to fetch game data for game {gameId}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var updatedGame = JsonConvert.DeserializeObject<MlbGameDetails>(content);

        if (updatedGame is null)
        {
            _logger.LogError("Failed to deserialize game data for game {GameId}", gameId.ToString());
            throw new Exception($"Failed to deserialize game data for game {gameId}");
        }

        return updatedGame;
    }
}
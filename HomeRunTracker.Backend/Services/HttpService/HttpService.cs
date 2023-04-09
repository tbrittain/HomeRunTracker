using System.Net;
using HomeRunTracker.Common.Models.Content;
using HomeRunTracker.Common.Models.Details;
using HomeRunTracker.Common.Models.Summary;
using Newtonsoft.Json;
using OneOf;
using OneOf.Types;

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

    public async Task<OneOf<MlbSchedule, HttpStatusCode, Error<string>>> FetchGames(DateTime dateTime)
    {
        var formattedDate = dateTime.ToString("yyyy-MM-dd");
        var url =
            $"https://statsapi.mlb.com/api/v1/schedule/games/?sportId=1&startDate={formattedDate}&endDate={formattedDate}";

        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return response.StatusCode;

        var content = await response.Content.ReadAsStringAsync();
        var schedule = JsonConvert.DeserializeObject<MlbSchedule>(content);

        if (schedule is not null) return schedule;
        
        return new Error<string>("Failed to deserialize MLB API response");
    }

    public async Task<OneOf<MlbGameDetails, HttpStatusCode, Error<string>>> FetchGameDetails(int gameId)
    {
        _logger.LogDebug("Fetching game data for game {GameId}", gameId.ToString());
        var url = $"https://statsapi.mlb.com/api/v1.1/game/{gameId}/feed/live";

        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return response.StatusCode;

        var content = await response.Content.ReadAsStringAsync();
        var game = JsonConvert.DeserializeObject<MlbGameDetails>(content);

        if (game is not null) return game;
        
        return new Error<string>($"Failed to deserialize game data for game {gameId}");
    }

    public async Task<OneOf<MlbGameContent, HttpStatusCode, Error<string>>> FetchGameContent(int gameId)
    {
        _logger.LogDebug("Fetching game content for game {GameId}", gameId.ToString());
        var url = $"https://statsapi.mlb.com/api/v1/game/{gameId}/content";
        
        var httpClient = _httpClientFactory.CreateClient();
        
        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return response.StatusCode;
        
        var content = await response.Content.ReadAsStringAsync();
        var gameContent = JsonConvert.DeserializeObject<MlbGameContent>(content);
        
        if (gameContent is not null) return gameContent;
        
        return new Error<string>($"Failed to deserialize game content for game {gameId}");
    }
}
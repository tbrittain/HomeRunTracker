using System.Net;
using System.Text.Json;
using HomeRunTracker.Core.Interfaces;
using HomeRunTracker.Core.Models.Content;
using HomeRunTracker.Core.Models.Details;
using HomeRunTracker.Core.Models.Schedule;
using HomeRunTracker.Infrastructure.MlbApiService.Mappings;
using HomeRunTracker.Infrastructure.MlbApiService.Models.Content;
using HomeRunTracker.Infrastructure.MlbApiService.Models.Details;
using HomeRunTracker.Infrastructure.MlbApiService.Models.Summary;
using Microsoft.Extensions.Logging;
using OneOf;
using OneOf.Types;

namespace HomeRunTracker.Infrastructure.MlbApiService.Services;

public class MlbApiService : IMlbApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MlbApiService> _logger;

    public MlbApiService(IHttpClientFactory httpClientFactory, ILogger<MlbApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<OneOf<ScheduleDto, HttpStatusCode, Error<string>>> FetchGames(DateTime dateTime)
    {
        var formattedDate = dateTime.ToString("yyyy-MM-dd");
        var url =
            $"https://statsapi.mlb.com/api/v1/schedule/games/?sportId=1&startDate={formattedDate}&endDate={formattedDate}";

        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return response.StatusCode;

        var content = await response.Content.ReadAsStringAsync();
        var schedule = JsonSerializer.Deserialize<MlbSchedule>(content);

        if (schedule is not null) return schedule.MapToScheduleDto();

        return new Error<string>("Failed to deserialize MLB API response");
    }

    public async Task<OneOf<GameDetailsDto, HttpStatusCode, Error<string>>> FetchGameDetails(int gameId)
    {
        _logger.LogDebug("Fetching game data for game {GameId}", gameId.ToString());
        var url = $"https://statsapi.mlb.com/api/v1.1/game/{gameId}/feed/live";

        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return response.StatusCode;

        var content = await response.Content.ReadAsStringAsync();
        var game = JsonSerializer.Deserialize<MlbGameDetails>(content);

        if (game is not null) return game.MapToGameDetailsDto();

        return new Error<string>($"Failed to deserialize game data for game {gameId}");
    }

    public async Task<OneOf<GameContentDto, HttpStatusCode, Error<string>>> FetchGameContent(int gameId)
    {
        _logger.LogDebug("Fetching game content for game {GameId}", gameId.ToString());
        var url = $"https://statsapi.mlb.com/api/v1/game/{gameId}/content";

        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return response.StatusCode;

        var content = await response.Content.ReadAsStringAsync();
        var gameContent = JsonSerializer.Deserialize<MlbGameContent>(content);

        if (gameContent is not null) return gameContent.MapToGameContentDto();

        return new Error<string>($"Failed to deserialize game content for game {gameId}");
    }
}
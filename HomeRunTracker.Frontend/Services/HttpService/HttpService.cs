using HomeRunTracker.Common.Models.Internal;
using Newtonsoft.Json;

namespace HomeRunTracker.Frontend.Services.HttpService;

public class HttpService : IHttpService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HttpService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<ScoringPlayRecord>> GetScoringPlaysAsync(DateTime? dateTime)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("http://localhost:5001");

        var endpoint = dateTime.HasValue
            ? $"/api/scoring-plays?date={dateTime.Value:yyyy-MM-dd}"
            : "/api/scoring-plays";

        var response = await httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var homeRuns = JsonConvert.DeserializeObject<List<ScoringPlayRecord>>(content);
        if (homeRuns is null)
            throw new InvalidOperationException("Unable to deserialize scoring plays");
        
        return homeRuns;
    }

    public async Task<List<GameScoreRecord>> GetGameScoresAsync(DateTime? dateTime)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("http://localhost:5001");
        
        var endpoint = dateTime.HasValue
            ? $"/api/game-scores?date={dateTime.Value:yyyy-MM-dd}"
            : "/api/game-scores";
        
        var response = await httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        
        var content = response.Content.ReadAsStringAsync();
        var gameScores = JsonConvert.DeserializeObject<List<GameScoreRecord>>(content.Result);
        if (gameScores is null)
            throw new InvalidOperationException("Unable to deserialize game scores");
        
        return gameScores;
    }
}
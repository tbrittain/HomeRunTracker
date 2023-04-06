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

    public async Task<List<HomeRunRecord>> GetHomeRunsAsync()
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("http://localhost:5001");
        
        var response = await httpClient.GetAsync("/api/home-runs");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var homeRuns = JsonConvert.DeserializeObject<List<HomeRunRecord>>(content);
        if (homeRuns is null)
            throw new InvalidOperationException("Unable to deserialize home runs");
        
        return homeRuns;
    }
}
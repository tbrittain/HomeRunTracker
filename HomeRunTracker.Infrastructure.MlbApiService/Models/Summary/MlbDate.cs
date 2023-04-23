using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Summary;

public class MlbDate
{
    public string Date { get; set; } = string.Empty;
    
    [JsonPropertyName("games")]
    public List<MlbGameSummary> Games { get; set; } = new();
}
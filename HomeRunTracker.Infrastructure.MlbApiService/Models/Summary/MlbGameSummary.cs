using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Summary;

public class MlbGameSummary
{
    [JsonPropertyName("gamePk")]
    public int Id { get; set; }
    
    public string Link { get; set; } = string.Empty;
    
    [JsonPropertyName("status")]
    public MlbGameStatus GameStatus { get; set; } = new();
}
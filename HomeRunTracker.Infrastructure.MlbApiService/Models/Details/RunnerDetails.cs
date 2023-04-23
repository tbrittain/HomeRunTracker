using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public class RunnerDetails
{
    [JsonPropertyName("responsiblePitcher")]
    public Pitcher Pitcher { get; set; } = new();
    
    [JsonPropertyName("isScoringEvent")]
    public bool IsScoringEvent { get; set; }
    
    [JsonPropertyName("earned")]
    public bool Earned { get; set; }
}
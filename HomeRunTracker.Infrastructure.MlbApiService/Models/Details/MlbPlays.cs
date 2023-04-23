using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public class MlbPlays
{
    [JsonPropertyName("scoringPlays")]
    public List<int> ScoringPlays { get; set; } = new();
    
    [JsonPropertyName("allPlays")]
    public List<MlbPlay> AllPlays { get; set; } = new();
}
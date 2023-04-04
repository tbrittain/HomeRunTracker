using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public class MlbPlays
{
    [JsonProperty("scoringPlays")]
    [Id(0)]
    public List<int> ScoringPlays { get; set; } = new();
    
    [JsonProperty("allPlays")]
    [Id(1)]
    public List<MlbPlay> AllPlays { get; set; } = new();
}
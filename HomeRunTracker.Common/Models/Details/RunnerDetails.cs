using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public class RunnerDetails
{
    [JsonProperty("responsiblePitcher")]
    [Id(0)]
    public Pitcher Pitcher { get; set; } = new();
    
    [JsonProperty("isScoringEvent")]
    [Id(1)]
    public bool IsScoringEvent { get; set; }
    
    [JsonProperty("earned")]
    [Id(2)]
    public bool Earned { get; set; }
}
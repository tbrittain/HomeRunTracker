using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public class MlbPlayRunner
{
    [JsonProperty("movement")]
    [Id(0)]
    public RunnerMovement Movement { get; set; } = new();
    
    [JsonProperty("details")]
    [Id(1)]
    public RunnerDetails Details { get; set; } = new();
}

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
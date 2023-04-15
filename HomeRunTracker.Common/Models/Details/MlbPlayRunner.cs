using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public class MlbPlayRunner
{
    [JsonProperty("movement")]
    [Id(0)]
    public RunnerMovement Movement { get; set; } = new();
}
using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public class MlbPlayCount
{
    [JsonProperty("outs")]
    [Id(0)]
    public int Outs { get; set; }
}
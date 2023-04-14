using HomeRunTracker.Common.Enums;
using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public class RunnerMovement
{
    [JsonProperty("originBase")]
    [Id(0)]
    public string? OriginBase { get; set; }
    
    public EBase Base => OriginBase switch
    {
        "1B" => EBase.First,
        "2B" => EBase.Second,
        "3B" => EBase.Third,
        _ => EBase.None
    };
}
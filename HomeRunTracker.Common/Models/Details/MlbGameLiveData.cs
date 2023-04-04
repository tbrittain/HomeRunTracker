using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public class MlbGameLiveData
{
    [JsonProperty("plays")]
    [Id(0)]
    public MlbPlays Plays { get; set; } = new();
}
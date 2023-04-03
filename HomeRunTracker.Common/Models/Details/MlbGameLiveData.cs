using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

public class MlbGameLiveData
{
    [JsonProperty("plays")]
    public MlbPlays Plays { get; set; } = new MlbPlays();
}
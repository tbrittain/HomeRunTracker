using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

public class MlbPlay
{
    [JsonProperty("result")]
    public PlayResult Result { get; set; } = new PlayResult();
    
    [JsonProperty("playEvents")]
    public List<PlayEvent> Events { get; set; } = new List<PlayEvent>();
}
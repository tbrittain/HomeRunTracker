using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Content;

[GenerateSerializer]
public class HighlightItem
{
    [JsonProperty("guid")]
    [Id(0)]
    public Guid? Guid { get; set; }

    [JsonProperty("playbacks")]
    [Id(1)]
    public List<HighlightPlayback> Playbacks { get; set; }
}
using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Content;

[GenerateSerializer]
public class MlbGameHighlightOverview
{
    [JsonProperty("highlights")]
    [Id(0)]
    public MlbGameHighlight Highlights { get; set; }
}

[GenerateSerializer]
public class MlbGameHighlight
{
    [JsonProperty("items")]
    [Id(0)]
    public List<HighlightItem> Items { get; set; }
}
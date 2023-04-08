using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Content;

[GenerateSerializer]
public class MlbGameContent
{
    [JsonProperty("highlights")]
    [Id(0)]
    public MlbGameHighlight Highlights { get; set; }
}
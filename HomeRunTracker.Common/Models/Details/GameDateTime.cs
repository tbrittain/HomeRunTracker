using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public class GameDateTime
{
    [JsonProperty("dateTime")]
    [Id(0)]
    public DateTime DateTime { get; set; }
}
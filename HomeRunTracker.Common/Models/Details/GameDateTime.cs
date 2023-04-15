using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public class GameDateTime
{
    [JsonProperty("dateTime")] [Id(0)]
    public string DateTime { get; set; } = string.Empty;
    
    public DateTimeOffset DateTimeOffset => DateTimeOffset.Parse(DateTime);
}
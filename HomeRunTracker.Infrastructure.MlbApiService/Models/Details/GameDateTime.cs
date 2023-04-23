using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public class GameDateTime
{
    [JsonPropertyName("dateTime")]
    public string DateTime { get; set; } = string.Empty;
    
    public DateTimeOffset DateTimeOffset => DateTimeOffset.Parse(DateTime);
}
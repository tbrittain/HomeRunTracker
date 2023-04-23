using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public class MlbPlayAbout
{
    [JsonPropertyName("inning")]
    public int Inning { get; set; }

    [JsonPropertyName("isTopInning")]
    public bool IsTopInning { get; set; }

    [JsonPropertyName("hasOut")]
    public bool HasOut { get; set; }
}
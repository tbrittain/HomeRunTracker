using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public record PlayEvent
{
    [JsonPropertyName("playId")]
    public Guid PlayId { get; set; }

    [JsonPropertyName("hitData")]
    public HitData? HitData { get; set; }
}
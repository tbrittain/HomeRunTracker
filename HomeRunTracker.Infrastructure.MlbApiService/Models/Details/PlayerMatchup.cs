using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public record PlayerMatchup
{
    [JsonPropertyName("batter")]
    public Batter Batter { get; set; } = new();

    [JsonPropertyName("pitcher")]
    public Pitcher Pitcher { get; set; } = new();
}
using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Summary;

public class MlbGameSummary
{
    [JsonPropertyName("gamePk")]
    public int Id { get; set; }

    [JsonPropertyName("status")]
    public MlbGameStatus GameStatus { get; set; } = new();
}
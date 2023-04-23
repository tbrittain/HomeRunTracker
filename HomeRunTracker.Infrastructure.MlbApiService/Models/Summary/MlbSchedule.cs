using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Summary;

public class MlbSchedule
{
    [JsonPropertyName("totalGames")]
    public int TotalGames { get; set; }

    [JsonPropertyName("dates")]
    public List<MlbDate> Dates { get; set; } = new();
}

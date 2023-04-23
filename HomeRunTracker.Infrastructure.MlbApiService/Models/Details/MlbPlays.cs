using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public class MlbPlays
{
    [JsonPropertyName("allPlays")]
    public List<MlbPlay> AllPlays { get; set; } = new();
}
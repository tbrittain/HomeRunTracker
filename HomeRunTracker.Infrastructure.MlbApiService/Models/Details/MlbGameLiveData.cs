using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public class MlbGameLiveData
{
    [JsonPropertyName("plays")]
    public MlbPlays Plays { get; set; } = new();
}
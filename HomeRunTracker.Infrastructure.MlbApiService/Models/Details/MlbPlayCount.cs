using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public class MlbPlayCount
{
    [JsonPropertyName("outs")]
    public int Outs { get; set; }
}
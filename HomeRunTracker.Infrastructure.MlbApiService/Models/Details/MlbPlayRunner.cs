using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public class MlbPlayRunner
{
    [JsonPropertyName("movement")]
    public RunnerMovement Movement { get; set; } = new();
    
    [JsonPropertyName("details")]
    public RunnerDetails Details { get; set; } = new();
}
using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public class HitData
{
    [JsonPropertyName("launchSpeed")]
    public double LaunchSpeed { get; set; }

    [JsonPropertyName("launchAngle")]
    public double LaunchAngle { get; set; }

    [JsonPropertyName("totalDistance")]
    public double TotalDistance { get; set; }
}
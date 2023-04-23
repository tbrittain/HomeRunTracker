using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public class TeamMatchup
{
    [JsonPropertyName("home")]
    public Team HomeTeam { get; set; } = new();

    [JsonPropertyName("away")]
    public Team AwayTeam { get; set; } = new();
}
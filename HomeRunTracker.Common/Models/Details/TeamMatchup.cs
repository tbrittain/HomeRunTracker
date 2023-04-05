using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public class TeamMatchup
{
    [JsonProperty("home")]
    [Id(0)]
    public Team HomeTeam { get; set; } = new();

    [JsonProperty("away")]
    [Id(1)]
    public Team AwayTeam { get; set; } = new();
}
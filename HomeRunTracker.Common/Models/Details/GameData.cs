using HomeRunTracker.Common.Models.Summary;
using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public class GameData
{
    [Id(0)]
    public MlbGameStatus Status { get; set; } = new();
    
    [JsonProperty("teams")]
    [Id(1)]
    public TeamMatchup TeamMatchup { get; set; } = new();
}
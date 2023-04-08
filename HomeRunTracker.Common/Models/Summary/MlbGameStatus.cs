using HomeRunTracker.Common.Enums;
using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Summary;

[GenerateSerializer]
public class MlbGameStatus
{
    [JsonProperty("detailedState")]
    [Id(0)]
    public string State { get; set; } = string.Empty;

    public EMlbGameStatus Status =>
        State switch
        {
            "Pre-Game" => EMlbGameStatus.PreGame,
            "Warmup" => EMlbGameStatus.Warmup,
            "Final" => EMlbGameStatus.Final,
            "In Progress" => EMlbGameStatus.InProgress,
            _ => EMlbGameStatus.Unknown
        };
}
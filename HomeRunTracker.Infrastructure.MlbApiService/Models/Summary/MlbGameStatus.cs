using System.Text.Json.Serialization;
using HomeRunTracker.SharedKernel.Enums;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Summary;

public class MlbGameStatus
{
    [JsonPropertyName("detailedState")]
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
using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Summary;

public class MlbGameStatus
{
    [JsonProperty("detailedState")]
    public string State { get; set; } = string.Empty;

    public EMlbGameStatus Status =>
        State switch
        {
            "Final" => EMlbGameStatus.Final,
            "In Progress" => EMlbGameStatus.InProgress,
            _ => throw new ArgumentOutOfRangeException()
        };
}

public enum EMlbGameStatus
{
    Final,
    InProgress,
    // likely more, but this is all we need for now
    // will need to handle Postponed
}
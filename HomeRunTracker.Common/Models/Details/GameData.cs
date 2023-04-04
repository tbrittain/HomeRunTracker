using HomeRunTracker.Common.Models.Summary;

namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public class GameData
{
    [Id(0)]
    public MlbGameStatus Status { get; set; } = new();
}
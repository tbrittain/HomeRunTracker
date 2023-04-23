using HomeRunTracker.SharedKernel.Enums;

namespace HomeRunTracker.Backend.Models.Schedule;

[GenerateSerializer]
public class GameSummary
{
    [Id(0)]
    public int Id { get; set; }

    [Id(1)]
    public EMlbGameStatus Status { get; set; }
}
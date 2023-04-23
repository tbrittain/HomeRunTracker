using HomeRunTracker.SharedKernel.Enums;

namespace HomeRunTracker.Core.Models.Schedule;

public class GameSummaryDto
{
    public int Id { get; set; }
    
    public EMlbGameStatus Status { get; set; }
}
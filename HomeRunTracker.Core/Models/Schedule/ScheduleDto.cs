namespace HomeRunTracker.Core.Models.Schedule;

public class ScheduleDto
{
    public int TotalGames { get; set; }

    public List<GameSummaryDto> Games { get; set; } = new();
}
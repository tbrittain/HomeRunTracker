namespace HomeRunTracker.Backend.Models.Schedule;

[GenerateSerializer]
public class Schedule
{
    [Id(0)]
    public int TotalGames { get; set; }

    [Id(1)]
    public List<GameSummary> Games { get; set; } = new();
}
namespace HomeRunTracker.Common.Models.Summary;

[GenerateSerializer]
public class MlbSchedule
{
    [Id(0)]
    public int TotalGames { get; set; }
    [Id(1)]
    public List<MlbDate> Dates { get; set; } = new List<MlbDate>();
}

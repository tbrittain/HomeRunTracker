namespace HomeRunTracker.Common.Models.Summary;

public class MlbSchedule
{
    public int TotalGames { get; set; }
    public List<MlbDate> Dates { get; set; } = new List<MlbDate>();
}

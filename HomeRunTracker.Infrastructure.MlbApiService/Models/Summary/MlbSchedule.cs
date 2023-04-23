namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Summary;

public class MlbSchedule
{
    public int TotalGames { get; set; }

    public List<MlbDate> Dates { get; set; } = new();
}

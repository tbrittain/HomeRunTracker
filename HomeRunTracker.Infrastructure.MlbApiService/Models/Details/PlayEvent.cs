namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public record PlayEvent
{
    public Guid PlayId { get; set; }

    public HitData? HitData { get; set; }
}
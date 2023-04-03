namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public class PlayEvent
{
    [Id(0)]
    public Guid PlayId { get; set; }
    
    [Id(1)]
    public HitData? HitData { get; set; }
}
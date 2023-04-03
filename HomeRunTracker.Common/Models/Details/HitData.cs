namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public class HitData
{
    [Id(0)]
    public int Index { get; set; }
    
    [Id(1)]
    public double LaunchSpeed { get; set; }
    
    [Id(2)]
    public double LaunchAngle { get; set; }
    
    [Id(3)]
    public double TotalDistance { get; set; }
}
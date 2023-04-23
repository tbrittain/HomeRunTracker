namespace HomeRunTracker.Backend.Models.Details;

[GenerateSerializer]
public class HitData
{
    [Id(0)]
    public double LaunchSpeed { get; set; }

    [Id(1)]
    public double LaunchAngle { get; set; }

    [Id(2)]
    public double TotalDistance { get; set; }
}
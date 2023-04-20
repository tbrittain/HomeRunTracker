namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public class MlbPlayAbout
{
    [Id(0)]
    public int Inning { get; set; }

    [Id(1)]
    public bool IsTopInning { get; set; }
    
    [Id(2)]
    public bool HasOut { get; set; }
}
using HomeRunTracker.SharedKernel.Enums;

namespace HomeRunTracker.Backend.Models.Details;

[GenerateSerializer]
public class Runner
{
    [Id(0)]
    public EBase Base { get; set; }

    [Id(1)]
    public Player ResponsiblePitcher { get; set; } = new();

    [Id(2)]
    public bool IsScoringEvent { get; set; }

    [Id(3)]
    public bool IsEarned { get; set; }
}
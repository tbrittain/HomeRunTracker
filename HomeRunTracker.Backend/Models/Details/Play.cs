using HomeRunTracker.SharedKernel.Enums;

namespace HomeRunTracker.Backend.Models.Details;

[GenerateSerializer]
public class Play
{
    [Id(0)]
    public Guid? Id { get; set; }

    [Id(1)]
    public string Description { get; set; } = string.Empty;

    [Id(2)]
    public int Rbi { get; set; }

    [Id(3)]
    public DateTimeOffset PlayEndTime { get; set; }

    [Id(4)]
    public EPlayResult Result { get; set; }

    [Id(5)]
    public HitData? HitData { get; set; }

    [Id(6)]
    public int Inning { get; set; }

    [Id(7)]
    public bool IsTopInning { get; set; }

    [Id(8)]
    public bool HasOut { get; set; }

    [Id(9)]
    public Player Batter { get; set; } = new();

    [Id(10)]
    public Player Pitcher { get; set; } = new();

    [Id(11)]
    public List<Runner> Runners { get; set; } = new();

    [Id(12)]
    public int Outs { get; set; }

    [Id(13)]
    public int HomeScoreBefore { get; set; }

    [Id(14)]
    public int AwayScoreBefore { get; set; }

    [Id(15)]
    public int HomeScoreAfter { get; set; }

    [Id(16)]
    public int AwayScoreAfter { get; set; }
}
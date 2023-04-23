using HomeRunTracker.SharedKernel.Enums;

namespace HomeRunTracker.Core.Models.Details;

public class PlayDto
{
    public Guid? Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public int Rbi { get; set; }

    public DateTimeOffset PlayEndTime { get; set; }

    public EPlayResult Result { get; set; }

    public HitDataDto? HitData { get; set; }

    public int Inning { get; set; }

    public bool IsTopInning { get; set; }

    public bool HasOut { get; set; }

    public PlayerDto Batter { get; set; } = new();

    public PlayerDto Pitcher { get; set; } = new();

    public List<RunnerDto> Runners { get; set; } = new();

    public int Outs { get; set; }

    public int HomeScoreBefore { get; set; }

    public int AwayScoreBefore { get; set; }

    public int HomeScoreAfter { get; set; }

    public int AwayScoreAfter { get; set; }
}
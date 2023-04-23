namespace HomeRunTracker.Backend.Models;

[GenerateSerializer]
public class GameScoreRecord
{
    [Id(0)]
    public int GameId { get; set; }

    [Id(1)]
    public int PitcherId { get; set; }

    [Id(2)]
    public string PitcherName { get; set; } = string.Empty;

    [Id(3)]
    public int TeamId { get; set; }

    [Id(4)]
    public string TeamName { get; set; } = string.Empty;

    [Id(5)]
    public int TeamIdAgainst { get; set; }

    [Id(6)]
    public string TeamNameAgainst { get; set; } = string.Empty;

    [Id(7)]
    public int Outs { get; set; }

    public int FullInningsPitched => (int) Math.Floor(Outs / 3.0);

    [Id(8)]
    public int Hits { get; set; }

    [Id(9)]
    public int Strikeouts { get; set; }

    [Id(10)]
    public int EarnedRuns { get; set; }

    [Id(11)]
    public int UnearnedRuns { get; set; }

    [Id(12)]
    public int Walks { get; set; }

    [Id(13)]
    public int GameScore { get; set; }
}
namespace HomeRunTracker.Common.Models.Internal;

[GenerateSerializer]
public record GameScoreRecord
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

    public int GameScore
    {
        get
        {
            var baseScore = 50;
            baseScore += Outs;

            if (FullInningsPitched >= 4)
            {
                baseScore += (2 * FullInningsPitched);
            }

            baseScore += Strikeouts;
            baseScore -= Hits;
            baseScore -= (4 * EarnedRuns);
            baseScore -= (2 * UnearnedRuns);
            baseScore -= Walks;

            return baseScore;
        }
    }
}
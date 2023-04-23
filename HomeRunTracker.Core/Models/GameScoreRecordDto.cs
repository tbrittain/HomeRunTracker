namespace HomeRunTracker.Core.Models;

public record GameScoreRecordDto
{
    public int GameId { get; set; }

    public int PitcherId { get; set; }

    public string PitcherName { get; set; } = string.Empty;

    public int TeamId { get; set; }

    public string TeamName { get; set; } = string.Empty;

    public int TeamIdAgainst { get; set; }

    public string TeamNameAgainst { get; set; } = string.Empty;

    public int Outs { get; set; }

    public int FullInningsPitched => (int) Math.Floor(Outs / 3.0);

    public int Hits { get; set; }

    public int Strikeouts { get; set; }

    public int EarnedRuns { get; set; }

    public int UnearnedRuns { get; set; }

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
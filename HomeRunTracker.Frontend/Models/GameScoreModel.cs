using System.Text;

namespace HomeRunTracker.Frontend.Models;

// ReSharper disable once ClassNeverInstantiated.Global
public class GameScoreModel
{
    public int GameId { get; set; }
    
    public int PitcherId { get; set; }
    
    public string PitcherName { get; set; } = string.Empty;
    
    public int TeamId { get; set; }
    
    public string TeamName { get; set; } = string.Empty;
    
    public int TeamIdAgainst { get; set; }
    
    public string TeamNameAgainst { get; set; } = string.Empty;
    
    public int Outs { get; set; }
    
    public int FullInningsPitched { get; set; }

    public string FormattedInningsPitched
    {
        get
        {
            var remainder = Outs - (FullInningsPitched * 3);
            return $"{FullInningsPitched}.{remainder}";
        }
    }
    
    public int Hits { get; set; }
    
    public int Strikeouts { get; set; }
    
    public int EarnedRuns { get; set; }
    
    public int UnearnedRuns { get; set; }
    
    public int Walks { get; set; }
    
    public int GameScore { get; set; }
    
    public string PitcherImageUrl => $"https://img.mlbstatic.com/mlb-photos/image/upload/d_people:generic:headshot:67:current.png/w_100,q_auto:best/v1/people/{PitcherId}/headshot/67/current";
    
    public string TeamImageUrl => $"https://midfield.mlbstatic.com/v1/team/{TeamId}/spots/72";
    
    public string TeamImageUrlAgainst => $"https://midfield.mlbstatic.com/v1/team/{TeamIdAgainst}/spots/72";

    public string FormattedGameScore
    {
        get
        {
            var sb = new StringBuilder();
            sb.Append(GameScore);

            switch (GameScore)
            {
                case >= 100:
                    sb.Append(" 🔥🔥🔥");
                    break;
                case >= 90:
                    sb.Append(" 🔥🔥");
                    break;
                case >= 80:
                    sb.Append(" 🔥");
                    break;
            }

            return sb.ToString();
        }
    }
    
    public string GameScoreColor => GetColorForGameScore().ToString();

    private RgbColor GetColorForGameScore()
    {
        switch (GameScore)
        {
            case <= 0:
                return new RgbColor(0, 0, 255);
            case >= 100:
                return new RgbColor(255, 0, 0);
            case 50:
                return new RgbColor(255, 255, 255);
            case < 50:
            {
                var percent = Math.Abs(GameScore - 50) / 50.0;
                var whiteness = (byte) (255 * (1 - percent));
                return new RgbColor(whiteness, whiteness, 255);
            }
            case > 50:
            {
                var percent = (GameScore - 50) / 50.0;
                var whiteness = (byte) (255 * (1 - percent));
                return new RgbColor(255, whiteness, whiteness);
            }
        }
    }
}
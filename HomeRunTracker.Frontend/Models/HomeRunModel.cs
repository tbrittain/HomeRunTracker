using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HomeRunTracker.Frontend.Models;

// ReSharper disable once ClassNeverInstantiated.Global
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class HomeRunModel
{
    public string Hash { get; set; } = string.Empty;
    
    public int GameId { get; set; }
    
    public DateTime DateTime { get; set; }
    
    public int BatterId { get; set; }
    
    public string BatterName { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public int Rbi { get; set; }
    
    public double LaunchSpeed { get; set; }
    
    public double LaunchAngle { get; set; }
    
    public double TotalDistance { get; set; }
    
    public int Inning { get; set; }
    
    public bool IsTopInning { get; set; }
    
    public string TeamName { get; set; } = string.Empty;
    
    public int TeamId { get; set; }
    
    public string TeamNameAgainst { get; set; } = string.Empty;
    
    public int TeamNameAgainstId { get; set; }
    
    public int PitcherId { get; set; }
    
    public string PitcherName { get; set; } = string.Empty;
    
    public string? HighlightUrl { get; set; }
    
    // ReSharper disable once UnusedMember.Global
    public string BatterImageUrl =>
        $"https://img.mlbstatic.com/mlb-photos/image/upload/d_people:generic:headshot:67:current.png/w_100,q_auto:best/v1/people/{BatterId}/headshot/67/current";

    // ReSharper disable once UnusedMember.Global
    public string BatterTeamImageUrl => $"https://midfield.mlbstatic.com/v1/team/{TeamId}/spots/72";

    public string FormattedDistance
    {
        get
        {
            var sb = new StringBuilder();
            sb.Append(TotalDistance);

            switch (TotalDistance)
            {
                case > 500:
                    sb.Append(" 🔥🔥🔥");
                    break;
                case > 480:
                    sb.Append(" 🔥🔥");
                    break;
                case > 460:
                    sb.Append(" 🔥");
                    break;
            }

            return sb.ToString();
        }
    }
    
    public string DistanceColor => GetColorForDistance(TotalDistance).ToString();
    
    private readonly record struct RgbColor(int R, int G, int B)
    {
        public override string ToString()
        {
            return $"rgb({R}, {G}, {B})";
        }
    }

    private static RgbColor GetColorForDistance(double distance)
    {
        switch (distance)
        {
            case 400:
                return new RgbColor(255, 255, 255);
            case < 350:
                return new RgbColor(0, 0, 255);
            case > 450:
                return new RgbColor(255, 0, 0);
            case < 400:
            {
                var percent = (distance - 350) / 50.0;
                var r = (int) (255 * percent);
                var g = (int) (255 * percent);
                return new RgbColor(r, g, 255);
            }
            case > 400:
            {
                var percent = (distance - 400) / 50.0;
                var g = (int) (255 * (1 - percent));
                var b = (int) (255 * (1 - percent));
                return new RgbColor(255, g, b);
            }
            default:
                return new RgbColor(0, 0, 0);
        }
    }
}
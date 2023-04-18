using System.Diagnostics.CodeAnalysis;
using System.Text;
using HomeRunTracker.Common.Enums;

namespace HomeRunTracker.Frontend.Models;

// ReSharper disable once ClassNeverInstantiated.Global
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class ScoringPlayModel
{
    public string Hash { get; set; } = string.Empty;

    public int GameId { get; set; }

    public DateTimeOffset DateTimeOffset { get; set; }

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

    public float LeverageIndex { get; set; }
    
    public int PlayResult { get; set; }
    
    public EPlayResult Result => (EPlayResult) PlayResult;

    // ReSharper disable once UnusedMember.Global
    public string BatterImageUrl =>
        $"https://img.mlbstatic.com/mlb-photos/image/upload/d_people:generic:headshot:67:current.png/w_100,q_auto:best/v1/people/{BatterId}/headshot/67/current";

    // ReSharper disable once UnusedMember.Global
    public string BatterTeamImageUrl => $"https://midfield.mlbstatic.com/v1/team/{TeamId}/spots/72";
    
    public string FormattedDateTimeOffset(TimeSpan currentOffset)
    {
        var localDateTime = DateTimeOffset.ToOffset(currentOffset);
        var formatted = localDateTime.ToString("h:mm tt");
        return formatted;
    }

    public string FormattedDistance
    {
        get
        {
            var sb = new StringBuilder();
            sb.Append(TotalDistance);
            sb.Append(" ft");

            switch (TotalDistance)
            {
                case >= 500:
                    sb.Append(" 🔥🔥🔥");
                    break;
                case >= 480:
                    sb.Append(" 🔥🔥");
                    break;
                case >= 460:
                    sb.Append(" 🔥");
                    break;
            }

            return sb.ToString();
        }
    }

    public string FormattedLaunchSpeed
    {
        get
        {
            var sb = new StringBuilder();
            sb.Append(LaunchSpeed);
            sb.Append(" mph");

            switch (LaunchSpeed)
            {
                case >= 116:
                    sb.Append(" 🔥🔥🔥");
                    break;
                case >= 113:
                    sb.Append(" 🔥🔥");
                    break;
                case >= 110:
                    sb.Append(" 🔥");
                    break;
            }

            return sb.ToString();
        }
    }

    public string FormattedLaunchAngle
    {
        get
        {
            var sb = new StringBuilder();
            sb.Append(LaunchAngle);
            sb.Append('°');

            return sb.ToString();
        }
    }

    public string FormattedLeverageIndex
    {
        get
        {
            var sb = new StringBuilder();
            sb.Append(LeverageIndex);

            switch (LeverageIndex)
            {
                case >= 6:
                    sb.Append(" ❗❗❗");
                    break;
                case >= (float) 4.5:
                    sb.Append(" ❗❗");
                    break;
                case >= 3:
                    sb.Append(" ❗");
                    break;
            }

            return sb.ToString();
        }
    }

    public string DistanceColor => GetColorForDistance().ToString();

    public string LaunchSpeedColor => GetColorForLaunchSpeed().ToString();

    public string LaunchAngleColor => GetColorForLaunchAngle().ToString();
    
    public string LeverageIndexColor => GetColorForLeverageIndex().ToString();

    private readonly record struct RgbColor(byte R, byte G, byte B)
    {
        public override string ToString()
        {
            return $"rgb({R}, {G}, {B})";
        }
    }

    private RgbColor GetColorForDistance()
    {
        switch (TotalDistance)
        {
            case 400:
                return new RgbColor(255, 255, 255);
            case < 350:
                return new RgbColor(0, 0, 255);
            case > 450:
                return new RgbColor(255, 0, 0);
            case < 400:
            {
                var percent = (TotalDistance - 350) / 50.0;
                var whiteness = (byte) (255 * percent);
                return new RgbColor(whiteness, whiteness, 255);
            }
            case > 400:
            {
                var percent = (TotalDistance - 400) / 50.0;
                var whiteness = (byte) (255 * (1 - percent));
                return new RgbColor(255, whiteness, whiteness);
            }
            default:
                return new RgbColor(0, 0, 0);
        }
    }

    private RgbColor GetColorForLaunchSpeed()
    {
        var roundedSpeed = (int) Math.Round(LaunchSpeed);
        switch (roundedSpeed)
        {
            case <= 90:
                return new RgbColor(0, 0, 255);
            case 100:
                return new RgbColor(255, 255, 255);
            case >= 110:
                return new RgbColor(255, 0, 0);
            case > 100 and < 110:
            {
                var percent = (LaunchSpeed - 100) / 10;
                var whiteness = (byte) (255 * (1 - percent));
                return new RgbColor(255, whiteness, whiteness);
            }
            case > 90 and < 100:
            {
                var percent = (LaunchSpeed - 90) / 10;
                var whiteness = (byte) (255 * percent);
                return new RgbColor(whiteness, whiteness, 255);
            }
        }
    }

    private RgbColor GetColorForLaunchAngle()
    {
        switch (LaunchAngle)
        {
            case 28:
                return new RgbColor(255, 0, 0);
            case <= 24 or >= 32:
                return new RgbColor(255, 255, 255);
            case < 28:
            {
                var percent = (LaunchAngle - 24) / 4;
                var whiteness = (byte) (255 * (1 - percent));
                return new RgbColor(255, whiteness, whiteness);
            }
            case > 28:
            {
                var percent = (LaunchAngle - 28) / 4;
                var whiteness = (byte) (255 * (1 - percent));
                return new RgbColor(255, whiteness, whiteness);
            }
            default:
                return new RgbColor(0, 0, 0);
        }
    }

    private RgbColor GetColorForLeverageIndex()
    {
        switch (LeverageIndex)
        {
            case 0:
                return new RgbColor(255, 255, 255);
            case > 3:
                return new RgbColor(255, 0, 0);
            default:
            {
                var percent = (LeverageIndex) / 3;
                var whiteness = (byte) (255 * (1 - percent));
                return new RgbColor(255, whiteness, whiteness);
            }
        }
    }
}
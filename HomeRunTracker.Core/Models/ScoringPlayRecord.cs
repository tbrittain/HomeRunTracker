using System.Security.Cryptography;
using System.Text;
using HomeRunTracker.SharedKernel.Enums;

namespace HomeRunTracker.Core.Models;

public record ScoringPlayRecord
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

    public string? HighlightUrl { get; set; } = string.Empty;

    public float LeverageIndex { get; set; }

    public EPlayResult PlayResult { get; set; }
    
    public static string GetHash(string description, int gameId)
    {
        var descriptionHash = MD5.HashData(Encoding.UTF8.GetBytes(description + gameId));
        var descriptionHashString = BitConverter.ToString(descriptionHash).Replace("-", string.Empty);
        return descriptionHashString;
    }
}
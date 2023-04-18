﻿using System.Security.Cryptography;
using System.Text;
using HomeRunTracker.Common.Enums;

namespace HomeRunTracker.Common.Models.Internal;

[GenerateSerializer]
public record ScoringPlayRecord
{
    [Id(0)]
    public string Hash { get; set; } = string.Empty;
    
    [Id(1)]
    public int GameId { get; set; }
    
    [Id(2)]
    public DateTimeOffset DateTimeOffset { get; set; }
    
    [Id(3)]
    public int BatterId { get; set; }
    
    [Id(4)]
    public string BatterName { get; set; } = string.Empty;
    
    [Id(5)]
    public string Description { get; set; } = string.Empty;
    
    [Id(6)]
    public int Rbi { get; set; }
    
    [Id(7)]
    public double LaunchSpeed { get; set; }
    
    [Id(8)]
    public double LaunchAngle { get; set; }
    
    [Id(9)]
    public double TotalDistance { get; set; }

    [Id(10)]
    public int Inning { get; set; }

    [Id(11)]
    public bool IsTopInning { get; set; }

    [Id(12)]
    public string TeamName { get; set; } = string.Empty;

    [Id(13)]
    public int TeamId { get; set; }

    [Id(14)]
    public string TeamNameAgainst { get; set; } = string.Empty;

    [Id(15)]
    public int TeamNameAgainstId { get; set; }

    [Id(16)]
    public int PitcherId { get; set; }

    [Id(17)]
    public string PitcherName { get; set; } = string.Empty;

    [Id(18)]
    public string? HighlightUrl { get; set; } = string.Empty;
    
    [Id(19)]
    public float LeverageIndex { get; set; }
    
    [Id(20)]
    public EPlayResult PlayResult { get; set; }
    
    public static string GetHash(string description, int gameId)
    {
        var descriptionHash = MD5.HashData(Encoding.UTF8.GetBytes(description + gameId));
        var descriptionHashString = BitConverter.ToString(descriptionHash).Replace("-", string.Empty);
        return descriptionHashString;
    }
}
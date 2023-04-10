﻿using System.Security.Cryptography;
using System.Text;

namespace HomeRunTracker.Common.Models.Internal;

[GenerateSerializer]
public record HomeRunRecord
{
    [Id(0)]
    public string Hash { get; set; } = string.Empty;
    
    [Id(1)]
    public int GameId { get; set; }
    
    [Id(2)]
    public DateTime DateTime { get; set; }
    
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

    // ReSharper disable once UnusedMember.Global
    public string BatterImageUrl =>
        $"https://img.mlbstatic.com/mlb-photos/image/upload/d_people:generic:headshot:67:current.png/w_100,q_auto:best/v1/people/{BatterId}/headshot/67/current";

    // ReSharper disable once UnusedMember.Global
    public string BatterTeamImageUrl => $"https://midfield.mlbstatic.com/v1/team/{TeamId}/spots/72";

    public static string GetHash(string description, int gameId)
    {
        var descriptionHash = MD5.HashData(Encoding.UTF8.GetBytes(description + gameId));
        var descriptionHashString = BitConverter.ToString(descriptionHash).Replace("-", string.Empty);
        return descriptionHashString;
    }
}
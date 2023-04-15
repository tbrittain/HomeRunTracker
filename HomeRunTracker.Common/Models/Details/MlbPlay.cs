﻿using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public record MlbPlay
{
    [JsonProperty("result")]
    [Id(0)]
    public PlayResult Result { get; set; } = new();
    
    [JsonProperty("playEvents")]
    [Id(1)]
    public List<PlayEvent> Events { get; set; } = new();
    
    [JsonProperty("matchup")]
    [Id(2)]
    public PlayerMatchup PlayerMatchup { get; set; } = new();
    
    [JsonProperty("playEndTime")]
    [Id(3)]
    public string PlayEndTime { get; set; } = string.Empty;
    
    public DateTime DateTime => DateTime.Parse(PlayEndTime);
    
    [JsonProperty("about")]
    [Id(4)]
    public MlbPlayAbout About { get; set; } = new();
    
    [JsonProperty("runners")]
    [Id(5)]
    public List<MlbPlayRunner> Runners { get; set; } = new();

    [JsonProperty("count")]
    [Id(6)]
    public MlbPlayCount Count { get; set; } = new();
    
    public (int homeScoreStart, int awayScoreStart) GetScoreStart()
    {
        var homeScoreStart = Result.HomeScore;
        var awayScoreStart = Result.AwayScore;

        var isTopInning = About.IsTopInning;

        if (Result.Rbi <= 0) return (homeScoreStart, awayScoreStart);
        if (isTopInning)
        {
            awayScoreStart -= Result.Rbi;
        }
        else
        {
            homeScoreStart -= Result.Rbi;
        }

        return (homeScoreStart, awayScoreStart);
    }
}
using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public record MlbPlay
{
    [JsonPropertyName("result")]
    public PlayResult Result { get; set; } = new();
    
    [JsonPropertyName("playEvents")]
    public List<PlayEvent> Events { get; set; } = new();
    
    [JsonPropertyName("matchup")]
    public PlayerMatchup PlayerMatchup { get; set; } = new();
    
    [JsonPropertyName("playEndTime")]
    public string PlayEndTime { get; set; } = string.Empty;
    
    public DateTimeOffset DateTimeOffset => DateTimeOffset.Parse(PlayEndTime);
    
    [JsonPropertyName("about")]
    public MlbPlayAbout About { get; set; } = new();
    
    [JsonPropertyName("runners")]
    public List<MlbPlayRunner> Runners { get; set; } = new();

    [JsonPropertyName("count")]
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
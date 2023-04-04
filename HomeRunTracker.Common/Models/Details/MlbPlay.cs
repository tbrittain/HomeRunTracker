using Newtonsoft.Json;

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
    public Matchup Matchup { get; set; } = new();
    
    [JsonProperty("playEndTime")]
    [Id(3)]
    public string PlayEndTime { get; set; } = string.Empty;
    
    public DateTime DateTime => DateTime.Parse(PlayEndTime);
}
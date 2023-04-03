using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public record MlbPlay
{
    [JsonProperty("result")]
    [Id(0)]
    public PlayResult Result { get; set; } = new PlayResult();
    
    [JsonProperty("playEvents")]
    [Id(1)]
    public List<PlayEvent> Events { get; set; } = new List<PlayEvent>();
    
    [JsonProperty("matchup")]
    [Id(2)]
    public Matchup Matchup { get; set; } = new Matchup();
}
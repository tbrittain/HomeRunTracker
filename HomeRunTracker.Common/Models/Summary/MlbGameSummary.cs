using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Summary;

[GenerateSerializer]
public class MlbGameSummary
{
    [JsonProperty("gamePk")]
    [Id(0)]
    public int Id { get; set; }

    [Id(1)]
    public string Link { get; set; } = string.Empty;
    
    [JsonProperty("status")]
    [Id(2)]
    public MlbGameStatus GameStatus { get; set; } = new MlbGameStatus();
    
    [JsonProperty("content")]
    [Id(3)]
    public MlbGameContent Content { get; set; } = new MlbGameContent();
}
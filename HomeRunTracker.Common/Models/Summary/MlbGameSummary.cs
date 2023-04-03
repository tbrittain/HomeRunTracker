using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Summary;

[GenerateSerializer]
public class MlbGameSummary
{
    [JsonProperty("gamePk")]
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    
    [JsonProperty("status")]
    public MlbGameStatus GameStatus { get; set; } = new MlbGameStatus();
    
    [JsonProperty("content")]
    public MlbGameContent Content { get; set; } = new MlbGameContent();
}
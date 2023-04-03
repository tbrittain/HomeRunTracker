using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Summary;

public class MlbDate
{
    public string Date { get; set; } = string.Empty;
    
    [JsonProperty("games")]
    public List<MlbGameSummary> Games { get; set; } = new List<MlbGameSummary>();
}
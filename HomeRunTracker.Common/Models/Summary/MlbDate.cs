using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Summary;

[GenerateSerializer]
public class MlbDate
{
    [Id(0)]
    public string Date { get; set; } = string.Empty;
    
    [JsonProperty("games")]
    [Id(1)]
    public List<MlbGameSummary> Games { get; set; } = new List<MlbGameSummary>();
}
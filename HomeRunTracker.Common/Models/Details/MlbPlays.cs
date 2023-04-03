using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

public class MlbPlays
{
    [JsonProperty("scoringPlays")]
    public List<int> ScoringPlays { get; set; } = new List<int>();
    
    [JsonProperty("allPlays")]
    public List<MlbPlay> AllPlays { get; set; } = new List<MlbPlay>();
}
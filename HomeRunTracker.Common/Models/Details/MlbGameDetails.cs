using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

public class MlbGameDetails
{
    [JsonProperty("gamePk")]
    public int Id { get; set; }
    
    public MlbGameLiveData LiveData { get; set; } = new MlbGameLiveData();
}
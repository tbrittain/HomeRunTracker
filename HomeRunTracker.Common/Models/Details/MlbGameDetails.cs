using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public class MlbGameDetails
{
    [JsonProperty("gamePk")]
    [Id(0)]
    public int Id { get; set; }

    [Id(1)]
    public MlbGameLiveData LiveData { get; set; } = new MlbGameLiveData();
    
    [JsonProperty("gameData")]
    [Id(2)]
    public GameData GameData { get; set; } = new GameData();
}
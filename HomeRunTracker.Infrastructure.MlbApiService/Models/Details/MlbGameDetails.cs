using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public class MlbGameDetails
{
    [JsonPropertyName("gamePk")]
    public int Id { get; set; }
    
    [JsonPropertyName("liveData")]
    public MlbGameLiveData LiveData { get; set; } = new();
    
    [JsonPropertyName("gameData")]
    public GameData GameData { get; set; } = new();
}
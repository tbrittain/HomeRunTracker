using System.Text.Json.Serialization;
using HomeRunTracker.Infrastructure.MlbApiService.Models.Summary;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public class GameData
{
    public MlbGameStatus Status { get; set; } = new();
    
    [JsonPropertyName("teams")]
    public TeamMatchup TeamMatchup { get; set; } = new();
    
    [JsonPropertyName("datetime")]
    public GameDateTime GameDateTime { get; set; } = new();
}
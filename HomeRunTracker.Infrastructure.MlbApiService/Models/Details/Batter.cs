using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public record Batter
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;
}
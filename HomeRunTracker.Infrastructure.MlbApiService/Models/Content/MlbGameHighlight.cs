using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Content;

public class MlbGameHighlight
{
    [JsonPropertyName("items")]
    public List<HighlightItem> Items { get; set; } = new();
}
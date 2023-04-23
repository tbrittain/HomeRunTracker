using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Content;

public class MlbGameContent
{
    [JsonPropertyName("highlights")]
    public MlbGameHighlightOverview? HighlightsOverview { get; set; }
}
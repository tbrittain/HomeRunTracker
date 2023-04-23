using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Content;

public class MlbGameHighlightOverview
{
    [JsonPropertyName("highlights")]
    public MlbGameHighlight? Highlights { get; set; }
}
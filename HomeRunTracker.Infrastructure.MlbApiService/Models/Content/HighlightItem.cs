using System.Text.Json.Serialization;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Content;

public class HighlightItem
{
    [JsonPropertyName("guid")]
    public string GuidString { get; set; } = string.Empty;

    public Guid? Guid
    {
        get
        {
            if (string.IsNullOrEmpty(GuidString)) return null;
            
            var success = System.Guid.TryParse(GuidString, out var guid);
            return success ? guid : null;
        }
    }

    [JsonPropertyName("playbacks")]
    public List<HighlightPlayback> Playbacks { get; set; } = new();

    [JsonPropertyName("keywordsAll")]
    public List<HighlightKeyword> Keywords { get; set; } = new();
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}
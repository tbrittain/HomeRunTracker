using System.Text.Json.Serialization;
using HomeRunTracker.SharedKernel.Enums;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Content;

public class HighlightPlayback
{
    [JsonPropertyName("name")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    public EPlaybackType PlaybackType => Type switch
    {
        "mp4Avc" => EPlaybackType.Mp4,
        "highBit" => EPlaybackType.HighBit,
        _ => EPlaybackType.Unknown
    };
}
using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Content;

[GenerateSerializer]
public class HighlightPlayback
{
    [JsonProperty("name")]
    [Id(0)]
    public string Type { get; set; } = string.Empty;
    
    [JsonProperty("url")]
    [Id(1)]
    public string Url { get; set; } = string.Empty;

    public EPlaybackType PlaybackType => Type switch
    {
        "mp4Avc" => EPlaybackType.Mp4,
        "highBit" => EPlaybackType.HighBit,
        _ => EPlaybackType.Unknown
    };
}

public enum EPlaybackType
{
    Unknown,
    Mp4,
    HighBit
}
using Newtonsoft.Json;
using System;

namespace HomeRunTracker.Common.Models.Content;

[GenerateSerializer]
public class HighlightItem
{
    [JsonProperty("guid")]
    [Id(0)]
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

    [JsonProperty("playbacks")]
    [Id(1)]
    public List<HighlightPlayback> Playbacks { get; set; } = new();
}
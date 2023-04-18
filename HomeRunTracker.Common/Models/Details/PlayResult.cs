using HomeRunTracker.Common.Enums;
using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public record PlayResult
{
    [JsonProperty("eventType")]
    [Id(0)]
    public string EventType { get; set; } = string.Empty;
    
    [JsonProperty("description")]
    [Id(1)]
    public string Description { get; set; } = string.Empty;
    
    [JsonProperty("rbi")]
    [Id(2)]
    public int Rbi { get; set; }

    public EPlayResult Result => EventType switch
    {
        "walk" => EPlayResult.Walk,
        "field_out" => EPlayResult.FieldOut,
        "force_out" => EPlayResult.ForceOut,
        "sac_fly" => EPlayResult.SacrificeFly,
        "strikeout" => EPlayResult.Strikeout,
        "single" => EPlayResult.Single,
        "double" => EPlayResult.Double,
        "triple" => EPlayResult.Triple,
        "home_run" => EPlayResult.HomeRun,
        _ => EPlayResult.Unknown
    };
    
    [JsonProperty("awayScore")]
    [Id(3)]
    public int AwayScore { get; set; }
    
    [JsonProperty("homeScore")]
    [Id(4)]
    public int HomeScore { get; set; }
}
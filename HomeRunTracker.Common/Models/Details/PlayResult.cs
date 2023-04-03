using Newtonsoft.Json;

namespace HomeRunTracker.Common.Models.Details;

public class PlayResult
{
    [JsonProperty("eventType")] 
    public string EventType { get; set; } = string.Empty;
    
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonProperty("rbi")]
    public int Rbi { get; set; }

    public EPlayResult Result => EventType switch
    {
        "walk" => EPlayResult.Walk,
        "field_out" => EPlayResult.FieldOut,
        "force_out" => EPlayResult.ForceOut,
        "strikeout" => EPlayResult.Strikeout,
        "single" => EPlayResult.Single,
        "double" => EPlayResult.Double,
        "triple" => EPlayResult.Triple,
        "home_run" => EPlayResult.HomeRun,
        _ => EPlayResult.Unknown
    };
}
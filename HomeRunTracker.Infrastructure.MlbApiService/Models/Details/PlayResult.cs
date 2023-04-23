using System.Text.Json.Serialization;
using HomeRunTracker.SharedKernel.Enums;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public record PlayResult
{
    [JsonPropertyName("eventType")]
    public string EventType { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("rbi")]
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

    [JsonPropertyName("awayScore")]
    public int AwayScore { get; set; }

    [JsonPropertyName("homeScore")]
    public int HomeScore { get; set; }
}
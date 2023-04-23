using System.Text.Json.Serialization;
using HomeRunTracker.SharedKernel.Enums;

namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public class RunnerMovement
{
    [JsonPropertyName("originBase")]
    public string? OriginBase { get; set; }

    public EBase Base => OriginBase switch
    {
        "1B" => EBase.First,
        "2B" => EBase.Second,
        "3B" => EBase.Third,
        _ => EBase.None
    };
}
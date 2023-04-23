namespace HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

public record PlayerMatchup
{
    public Batter Batter { get; set; } = new();

    public Pitcher Pitcher { get; set; } = new();
}
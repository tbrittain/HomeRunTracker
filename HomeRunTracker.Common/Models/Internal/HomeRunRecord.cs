namespace HomeRunTracker.Common.Models.Internal;

[GenerateSerializer]
public record HomeRunRecord
{
    [Id(0)]
    public Guid Id { get; set; }
    [Id(1)]
    public int GameId { get; set; }
    [Id(2)]
    public DateTime DateTime { get; set; }
    [Id(3)]
    public int BatterId { get; set; }
    [Id(4)]
    public string BatterName { get; set; } = string.Empty;
    [Id(5)]
    public string Description { get; set; } = string.Empty;
    [Id(6)]
    public int Rbi { get; set; }
    [Id(7)]
    public double LaunchSpeed { get; set; }
    [Id(8)]
    public double LaunchAngle { get; set; }
    [Id(9)]
    public double TotalDistance { get; set; }
}
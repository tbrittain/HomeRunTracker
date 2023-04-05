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

    [Id(10)]
    public int Inning { get; set; }

    [Id(11)]
    public bool IsTopInning { get; set; }

    // TODO: Handle getting the teams, as the team players come back as a list of player objects with dynamic key names
    [Id(12)]
    public string TeamName { get; set; } = string.Empty;

    [Id(13)]
    public int TeamId { get; set; }

    [Id(14)]
    public string TeamNameAgainst { get; set; } = string.Empty;

    [Id(15)]
    public int TeamNameAgainstId { get; set; }

    [Id(16)]
    public int PitcherId { get; set; }

    [Id(17)]
    public string PitcherName { get; set; } = string.Empty;
}
using HomeRunTracker.SharedKernel.Enums;

namespace HomeRunTracker.Backend.Models.Details;

[GenerateSerializer]
public class GameDetails
{
    public int Id { get; set; }
    
    public EMlbGameStatus Status { get; set; }
    
    public DateTimeOffset GameStartTime { get; set; }
    
    public Team HomeTeam { get; set; } = new();
    
    public Team AwayTeam { get; set; } = new();
    
    public List<Play> Plays { get; set; } = new();
}
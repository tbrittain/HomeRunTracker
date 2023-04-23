using HomeRunTracker.SharedKernel.Enums;

namespace HomeRunTracker.Core.Models.Details;

public class GameDetailsDto
{
    public int Id { get; set; }
    
    public EMlbGameStatus Status { get; set; }
    
    public DateTimeOffset GameStartTime { get; set; }
    
    public TeamDto HomeTeam { get; set; } = new();
    
    public TeamDto AwayTeam { get; set; } = new();
    
    public List<PlayDto> Plays { get; set; } = new();
}
using HomeRunTracker.SharedKernel.Enums;

namespace HomeRunTracker.Core.Models.Details;

public class RunnerDto
{
    public EBase Base { get; set; }
    
    public PlayerDto ResponsiblePitcher { get; set; } = new();
    
    public bool IsScoringEvent { get; set; }
    
    public bool IsEarned { get; set; }
}
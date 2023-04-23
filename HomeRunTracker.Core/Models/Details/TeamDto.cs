namespace HomeRunTracker.Core.Models.Details;

public record TeamDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
}
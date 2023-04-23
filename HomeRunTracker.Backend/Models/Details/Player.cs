namespace HomeRunTracker.Backend.Models.Details;

[GenerateSerializer]
public class Player
{
    public int Id { get; set; }
    
    public string FullName { get; set; } = string.Empty;
}
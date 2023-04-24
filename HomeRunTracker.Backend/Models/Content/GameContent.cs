namespace HomeRunTracker.Backend.Models.Content;

[GenerateSerializer]
public class GameContent
{
    [Id(0)]
    public List<Highlight> Highlights { get; set; } = new();
}
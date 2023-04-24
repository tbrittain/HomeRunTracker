namespace HomeRunTracker.Backend.Models.Content;

[GenerateSerializer]
public class Highlight
{
    [Id(0)]
    public Guid? Guid { get; set; }

    [Id(1)]
    public List<HighlightPlayback> Playbacks { get; set; } = new();
    
    [Id(2)]
    public List<HighlightKeyword> Keywords { get; set; } = new();

    [Id(3)]
    public string Title { get; set; } = string.Empty;

    [Id(4)]
    public string Description { get; set; } = string.Empty;
}
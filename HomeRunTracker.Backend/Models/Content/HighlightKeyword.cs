namespace HomeRunTracker.Backend.Models.Content;

[GenerateSerializer]
public class HighlightKeyword
{
    [Id(0)]
    public string Type { get; set; } = string.Empty;

    [Id(1)]
    public string Value { get; set; } = string.Empty;
}
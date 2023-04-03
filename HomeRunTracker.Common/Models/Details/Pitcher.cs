namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public record Pitcher
{
    [Id(0)]
    public int Id { get; set; }
    
    [Id(1)]
    public string FullName { get; set; } = string.Empty;
    
    [Id(2)]
    public string Link { get; set; } = string.Empty;
}
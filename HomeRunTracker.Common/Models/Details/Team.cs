namespace HomeRunTracker.Common.Models.Details;

[GenerateSerializer]
public class Team
{
    [Id(0)]
    public int Id { get; set; }

    [Id(1)]
    public string Name { get; set; } = string.Empty;
}
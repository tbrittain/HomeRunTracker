namespace HomeRunTracker.Core.Models.Content;

public class HighlightKeywordDto
{
    public const string TaxonomyType = "taxonomy";
    public const string PlayerIdType = "player_id";
    public const string StartingPitcherHighlightValue = "highlight-reel-starting-pitching";
    public const string GeneralPitcherValue = "pitching";
    public const string GeneralHighlightValue = "highlight";
    
    public string Type { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}
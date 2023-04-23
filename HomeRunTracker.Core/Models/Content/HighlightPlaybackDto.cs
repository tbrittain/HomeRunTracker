using HomeRunTracker.SharedKernel.Enums;

namespace HomeRunTracker.Core.Models.Content;

public class HighlightPlaybackDto
{
    public EPlaybackType PlaybackType { get; set; }
    
    public string Url { get; set; } = string.Empty;
}
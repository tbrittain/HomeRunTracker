using HomeRunTracker.SharedKernel.Enums;

namespace HomeRunTracker.Backend.Models.Content;

[GenerateSerializer]
public class HighlightPlayback
{
    [Id(0)]
    public EPlaybackType PlaybackType { get; set; }

    [Id(1)]
    public string Url { get; set; } = string.Empty;
}
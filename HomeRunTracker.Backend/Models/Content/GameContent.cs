using HomeRunTracker.SharedKernel.Enums;

namespace HomeRunTracker.Backend.Models.Content;

[GenerateSerializer]
public class GameContent
{
    [Id(0)]
    public List<Highlight> Highlights { get; set; } = new();
}

[GenerateSerializer]
public class Highlight
{
    [Id(0)]
    public Guid? Guid { get; set; }

    [Id(1)]
    public List<HighlightPlayback> Playbacks { get; set; } = new();
}

[GenerateSerializer]
public class HighlightPlayback
{
    [Id(0)]
    public EPlaybackType PlaybackType { get; set; }

    [Id(1)]
    public string Url { get; set; } = string.Empty;
}
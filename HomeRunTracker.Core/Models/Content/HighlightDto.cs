﻿namespace HomeRunTracker.Core.Models.Content;

public class HighlightDto
{
    public Guid? Guid { get; set; }

    public List<HighlightPlaybackDto> Playbacks { get; set; } = new();

    public List<HighlightKeywordDto> Keywords { get; set; } = new();
}
using HomeRunTracker.Core.Models.Content;
using HomeRunTracker.Infrastructure.MlbApiService.Models.Content;

namespace HomeRunTracker.Infrastructure.MlbApiService.Mappings;

public static class MlbGameContentMapping
{
    public static GameContentDto MapToGameContentDto(this MlbGameContent mlbGameContent)
    {
        return new GameContentDto
        {
            Highlights = mlbGameContent.HighlightsOverview?.Highlights?.Items?
                .Select(highlightItem => new HighlightDto
                {
                    Guid = highlightItem.Guid,
                    Playbacks = highlightItem.Playbacks.Select(playback => new HighlightPlaybackDto
                    {
                        PlaybackType = playback.PlaybackType,
                        Url = playback.Url
                    }).ToList()
                })
                .ToList() ?? new List<HighlightDto>()
        };
    }
}
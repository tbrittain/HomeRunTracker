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
                    Title = highlightItem.Title,
                    Description = highlightItem.Description,
                    Keywords = highlightItem.Keywords.Select(keyword => new HighlightKeywordDto
                    {
                        Type = keyword.Type,
                        Value = keyword.Value
                    }).ToList(),
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
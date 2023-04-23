using MediatR;

namespace HomeRunTracker.Core.Actions.ScoringPlays.Notifications;

public class ScoringPlayUpdatedNotification : INotification
{
    public ScoringPlayUpdatedNotification(string homeRunHash, int gameId, DateTimeOffset gameStartTime, string highlightUrl)
    {
        HomeRunHash = homeRunHash;
        GameId = gameId;
        GameStartTime = gameStartTime;
        HighlightUrl = highlightUrl;
    }
    
    public string HomeRunHash { get; }

    public int GameId { get; }

    public DateTimeOffset GameStartTime { get; }

    public string HighlightUrl { get; }
}
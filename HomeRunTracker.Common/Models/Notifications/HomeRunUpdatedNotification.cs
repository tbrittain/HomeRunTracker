using MediatR;

namespace HomeRunTracker.Common.Models.Notifications;

[GenerateSerializer]
public class HomeRunUpdatedNotification : INotification
{
    public HomeRunUpdatedNotification(string homeRunHash, int gameId, DateTimeOffset gameStartTime, string highlightUrl)
    {
        HomeRunHash = homeRunHash;
        GameId = gameId;
        GameStartTime = gameStartTime;
        HighlightUrl = highlightUrl;
    }

    [Id(0)]
    public string HomeRunHash { get; }
    
    [Id(1)]
    public int GameId { get; }
    
    [Id(2)]
    public DateTimeOffset GameStartTime { get; }

    [Id(3)]
    public string HighlightUrl { get; }
}
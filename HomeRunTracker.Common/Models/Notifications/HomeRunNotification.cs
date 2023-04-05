using HomeRunTracker.Common.Models.Internal;
using MediatR;

namespace HomeRunTracker.Common.Models.Notifications;

public class HomeRunNotification : INotification
{
    public HomeRunNotification(int gameId, HomeRunRecord homeRun)
    {
        GameId = gameId;
        HomeRun = homeRun;
    }
    
    public int GameId { get; }
    
    public HomeRunRecord HomeRun { get; }
}
using HomeRunTracker.Common.Models.Internal;
using MediatR;

namespace HomeRunTracker.Common.Models.Notifications;

[GenerateSerializer]
public class HomeRunNotification : INotification
{
    public HomeRunNotification(int gameId, HomeRunRecord homeRun)
    {
        GameId = gameId;
        HomeRun = homeRun;
    }

    [Id(0)]
    public int GameId { get; }

    [Id(1)]
    public HomeRunRecord HomeRun { get; }
}
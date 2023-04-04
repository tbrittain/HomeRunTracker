using MediatR;

namespace HomeRunTracker.Common.Models.Notifications;

public class GameStoppedNotification : INotification
{
    public GameStoppedNotification(int gameId)
    {
        GameId = gameId;
    }

    public int GameId { get; }
}
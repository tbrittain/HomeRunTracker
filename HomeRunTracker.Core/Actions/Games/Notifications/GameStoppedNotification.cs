using MediatR;

namespace HomeRunTracker.Core.Actions.Games.Notifications;

public class GameStoppedNotification : INotification
{
    public GameStoppedNotification(int gameId)
    {
        GameId = gameId;
    }

    public int GameId { get; }
}
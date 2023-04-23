using MediatR;

namespace HomeRunTracker.Backend.Actions.Game.Notifications;

[GenerateSerializer]
public class GameStoppedNotification : INotification
{
    public GameStoppedNotification(int gameId)
    {
        GameId = gameId;
    }

    [Id(0)]
    public int GameId { get; }
}
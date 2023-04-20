using HomeRunTracker.Common.Models.Internal;
using MediatR;

namespace HomeRunTracker.Common.Models.Notifications;

[GenerateSerializer]
public class GameScoreNotification : INotification
{
    public GameScoreNotification(int gameId, DateTimeOffset gameStartTime, GameScoreRecord gameScoreRecord)
    {
        GameId = gameId;
        GameStartTime = gameStartTime;
        GameScoreRecord = gameScoreRecord;
    }
    
    [Id(0)]
    public int GameId { get; }

    [Id(1)]
    public DateTimeOffset GameStartTime { get; }

    [Id(2)]
    public GameScoreRecord GameScoreRecord { get; }
}
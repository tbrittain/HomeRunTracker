using HomeRunTracker.Core.Models;
using MediatR;

namespace HomeRunTracker.Core.Actions.GameScores.Notifications;

public class GameScoreNotification : INotification
{
    public GameScoreNotification(int gameId, DateTimeOffset gameStartTime, GameScoreRecord gameScoreRecord)
    {
        GameId = gameId;
        GameStartTime = gameStartTime;
        GameScoreRecord = gameScoreRecord;
    }
    
    public int GameId { get; }
    
    public DateTimeOffset GameStartTime { get; }
    
    public GameScoreRecord GameScoreRecord { get; }
}
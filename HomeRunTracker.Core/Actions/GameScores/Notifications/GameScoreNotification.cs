using HomeRunTracker.Core.Models;
using MediatR;

namespace HomeRunTracker.Core.Actions.GameScores.Notifications;

public class GameScoreNotification : INotification
{
    public GameScoreNotification(int gameId, DateTimeOffset gameStartTime, GameScoreRecordDto gameScoreRecordDto)
    {
        GameId = gameId;
        GameStartTime = gameStartTime;
        GameScoreRecordDto = gameScoreRecordDto;
    }
    
    public int GameId { get; }
    
    public DateTimeOffset GameStartTime { get; }
    
    public GameScoreRecordDto GameScoreRecordDto { get; }
}
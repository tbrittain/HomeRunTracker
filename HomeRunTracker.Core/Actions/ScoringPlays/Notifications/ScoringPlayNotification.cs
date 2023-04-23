using HomeRunTracker.Core.Models;
using MediatR;

namespace HomeRunTracker.Core.Actions.ScoringPlays.Notifications;

public class ScoringPlayNotification : INotification
{
    public ScoringPlayNotification(int gameId, DateTimeOffset gameStartTime, ScoringPlayRecord scoringPlay)
    {
        GameId = gameId;
        GameStartTime = gameStartTime;
        ScoringPlay = scoringPlay;
    }
    
    public int GameId { get; }
    
    public DateTimeOffset GameStartTime { get; }
    
    public ScoringPlayRecord ScoringPlay { get; }
}
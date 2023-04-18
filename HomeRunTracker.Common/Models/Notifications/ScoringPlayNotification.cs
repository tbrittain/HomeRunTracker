using HomeRunTracker.Common.Models.Internal;
using MediatR;

namespace HomeRunTracker.Common.Models.Notifications;

[GenerateSerializer]
public class ScoringPlayNotification : INotification
{
    public ScoringPlayNotification(int gameId, DateTimeOffset gameStartTime, ScoringPlayRecord scoringPlay)
    {
        GameId = gameId;
        GameStartTime = gameStartTime;
        ScoringPlay = scoringPlay;
    }

    [Id(0)]
    public int GameId { get; }
    
    [Id(1)]
    public DateTimeOffset GameStartTime { get; }

    [Id(2)]
    public ScoringPlayRecord ScoringPlay { get; }
}
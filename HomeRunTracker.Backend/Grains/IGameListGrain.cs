using HomeRunTracker.Backend.Actions.GameScore.Notifications;
using HomeRunTracker.Backend.Actions.ScoringPlay.Notifications;
using HomeRunTracker.Backend.Models;

namespace HomeRunTracker.Backend.Grains;

public interface IGameListGrain : IGrainWithIntegerKey
{
    Task<List<ScoringPlayRecord>> GetScoringPlays(DateTime dateTime);
    Task<List<GameScoreRecord>> GetGameScores(DateTime dateTime);

    Task PublishScoringPlay(ScoringPlayNotification notification);
    Task PublishScoringPlayUpdated(ScoringPlayUpdatedNotification notification);
    Task PublishGameScore(GameScoreNotification notification);
}
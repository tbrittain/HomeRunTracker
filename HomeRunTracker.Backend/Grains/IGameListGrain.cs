using HomeRunTracker.Common.Models.Internal;
using HomeRunTracker.Common.Models.Notifications;

namespace HomeRunTracker.Backend.Grains;

public interface IGameListGrain : IGrainWithIntegerKey
{
    Task<List<ScoringPlayRecord>> GetScoringPlays(DateTime dateTime);
    
    Task PublishScoringPlay(ScoringPlayNotification notification);
    Task PublishScoringPlayUpdated(ScoringPlayUpdatedNotification notification);
}
using HomeRunTracker.Common.Models.Internal;
using HomeRunTracker.Common.Models.Notifications;

namespace HomeRunTracker.Backend.Grains;

public interface IGameListGrain : IGrainWithIntegerKey
{
    Task<List<ScoringPlayRecord>> GetHomeRuns(DateTime dateTime);
    
    Task PublishHomeRun(ScoringPlayNotification notification);
    Task PublishHomeRunUpdated(ScoringPlayUpdatedNotification notification);
}
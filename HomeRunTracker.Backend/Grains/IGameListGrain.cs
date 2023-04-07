using HomeRunTracker.Common.Models.Internal;
using HomeRunTracker.Common.Models.Notifications;

namespace HomeRunTracker.Backend.Grains;

public interface IGameListGrain : IGrainWithIntegerKey
{
    Task<List<HomeRunRecord>> GetHomeRuns(DateTime dateTime);
    
    Task PublishHomeRun(HomeRunNotification notification);
}
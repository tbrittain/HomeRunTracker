using HomeRunTracker.Common.Models.Internal;
using HomeRunTracker.Common.Models.Notifications;

namespace HomeRunTracker.Backend.Grains;

public interface IGameListGrain : IGrainWithIntegerKey
{
    Task<List<HomeRunRecord>> GetHomeRunsAsync(DateTime dateTime);
    
    Task PublishHomeRunAsync(HomeRunNotification notification);
}
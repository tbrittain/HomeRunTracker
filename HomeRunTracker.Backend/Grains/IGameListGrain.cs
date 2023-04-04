using HomeRunTracker.Common.Models.Internal;

namespace HomeRunTracker.Backend.Grains;

public interface IGameListGrain : IGrainWithIntegerKey
{
    Task<List<HomeRunRecord>> GetHomeRunsAsync();
}
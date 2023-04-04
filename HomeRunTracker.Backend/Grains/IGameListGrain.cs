using HomeRunTracker.Common.Models.Internal;

namespace HomeRunTracker.Backend.Grains;

public interface IGameListGrain : IGrainWithGuidKey
{
    Task<List<HomeRunRecord>> GetHomeRunsAsync();
}
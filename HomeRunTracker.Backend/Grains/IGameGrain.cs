using HomeRunTracker.Common;

namespace HomeRunTracker.Backend.Grains;

public interface IGameGrain : IGrainWithIntegerKey
{
    Task InitializeAsync(Post game);
    Task<Post> GetGameAsync();
    Task StopAsync();
}
using HomeRunTracker.Common.Models.Details;

namespace HomeRunTracker.Backend.Grains;

public interface IGameGrain : IGrainWithIntegerKey
{
    Task<MlbGameDetails> GetGameAsync();
    Task StopAsync();
}
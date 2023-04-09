using HomeRunTracker.Common.Models.Content;
using HomeRunTracker.Common.Models.Details;

namespace HomeRunTracker.Backend.Grains;

public interface IGameGrain : IGrainWithIntegerKey
{
    Task<MlbGameDetails> GetGame();
    Task<MlbGameContent> GetGameContent();
    Task Stop();
}
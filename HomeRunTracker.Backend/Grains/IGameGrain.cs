using HomeRunTracker.Common.Models.Details;
using HomeRunTracker.Common.Models.Internal;

namespace HomeRunTracker.Backend.Grains;

public interface IGameGrain : IGrainWithIntegerKey
{
    Task<MlbGameDetails> GetGame();
    Task Stop();
    Task<List<ScoringPlayRecord>> GetHomeRuns();
}
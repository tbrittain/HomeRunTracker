using HomeRunTracker.Common.Models.Details;
using HomeRunTracker.Common.Models.Summary;

namespace HomeRunTracker.Backend.Grains;

public interface IGameGrain : IGrainWithIntegerKey
{
    Task<int> InitializeAsync(MlbGameSummary gameId);
    Task<MlbGameDetails> GetGameAsync();
    Task StopAsync();
    event EventHandler<GameGrain.GameStoppedEventArgs> OnGameStopped;
}
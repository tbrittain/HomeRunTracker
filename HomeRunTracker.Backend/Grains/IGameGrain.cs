using HomeRunTracker.Core.Models;

namespace HomeRunTracker.Backend.Grains;

public interface IGameGrain : IGrainWithIntegerKey
{
    Task<MlbGameDetails> GetGame();
    Task Stop();
    // TODO: Need to return lists of new models that have GenerateSerializerAttribute
    Task<List<ScoringPlayRecord>> GetScoringPlays();
    Task<List<GameScoreRecord>> GetGameScores();
}
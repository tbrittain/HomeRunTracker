using HomeRunTracker.Backend.Models;
using HomeRunTracker.Backend.Models.Details;

namespace HomeRunTracker.Backend.Grains;

public interface IGameGrain : IGrainWithIntegerKey
{
    Task<GameDetails> GetGame();
    Task Stop();
    // TODO: Need to return lists of new models that have GenerateSerializerAttribute
    Task<List<ScoringPlayRecord>> GetScoringPlays();
    Task<List<GameScoreRecord>> GetGameScores();
}
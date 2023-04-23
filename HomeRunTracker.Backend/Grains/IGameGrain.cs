using HomeRunTracker.Backend.Models;
using HomeRunTracker.Backend.Models.Details;

namespace HomeRunTracker.Backend.Grains;

public interface IGameGrain : IGrainWithIntegerKey
{
    Task<GameDetails> GetGame();
    Task Stop();
    Task<List<ScoringPlayRecord>> GetScoringPlays();
    Task<List<GameScoreRecord>> GetGameScores();
}
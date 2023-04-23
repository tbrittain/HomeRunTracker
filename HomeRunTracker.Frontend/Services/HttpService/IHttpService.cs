using HomeRunTracker.Backend.Models;

namespace HomeRunTracker.Frontend.Services.HttpService;

public interface IHttpService
{
    Task<List<ScoringPlayRecord>> GetScoringPlaysAsync(DateTime? dateTime);
    Task<List<GameScoreRecord>> GetGameScoresAsync(DateTime? dateTime);
}
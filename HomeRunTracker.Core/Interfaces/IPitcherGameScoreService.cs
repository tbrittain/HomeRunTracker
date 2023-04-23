using HomeRunTracker.Core.Models;
using HomeRunTracker.Core.Models.Details;

namespace HomeRunTracker.Core.Interfaces;

public interface IPitcherGameScoreService
{
    public HashSet<GameScoreRecord> GetPitcherGameScores(GameDetailsDto gameDetails);
}
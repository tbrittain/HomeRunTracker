using HomeRunTracker.Backend.Extensions;
using HomeRunTracker.Common.Enums;
using HomeRunTracker.Common.Models.Details;
using HomeRunTracker.Common.Models.Internal;

namespace HomeRunTracker.Backend.Services;

public class PitcherGameScoreService
{
    public HashSet<GameScoreRecord> GetPitcherGameScores(MlbGameDetails gameDetails)
    {
        var pitcherGameScores = new HashSet<GameScoreRecord>();
        
        var playGroupings = gameDetails.LiveData.Plays.AllPlays
            .GroupBy(x => x.PlayerMatchup.Pitcher);

        foreach (var grouping in playGroupings)
        {
            var pitcher = grouping.Key;
            var plays = grouping.ToList();

            var outs = plays
                .Sum(x => x.About.HasOut ? 1 : 0);
            var numHits = plays
                .Sum(x => x.Result.Result.IsHit() ? 1 : 0);
            var numStrikeOuts = plays
                .Sum(x => x.Result.Result is EPlayResult.Strikeout ? 1 : 0);
            var earnedRuns = plays
                .Select(x => x.Runners
                    .Sum(y => y.Details is {IsScoringEvent: true, Earned: true} ? 1 : 0)).Sum();
            var unearnedRuns = plays
                .Select(x => x.Runners
                    .Sum(y => y.Details is {IsScoringEvent: true, Earned: false} ? 1 : 0)).Sum();
            var walks = plays
                .Sum(x => x.Result.Result is EPlayResult.Walk ? 1 : 0);
            
            var (batterTeamId, pitcherTeamId, batterTeamName, pitcherTeamName, _) =
                new PlayTeams(plays[0], gameDetails);
            
            var gameScore = new GameScoreRecord
            {
                GameId = gameDetails.Id,
                PitcherId = pitcher.Id,
                PitcherName = pitcher.FullName,
                TeamId = pitcherTeamId,
                TeamName = pitcherTeamName,
                TeamIdAgainst = batterTeamId,
                TeamNameAgainst = batterTeamName,
                Outs = outs,
                Hits = numHits,
                Strikeouts = numStrikeOuts,
                EarnedRuns = earnedRuns,
                UnearnedRuns = unearnedRuns,
                Walks = walks
            };
            
            pitcherGameScores.Add(gameScore);
        }

        return pitcherGameScores;
    }
}
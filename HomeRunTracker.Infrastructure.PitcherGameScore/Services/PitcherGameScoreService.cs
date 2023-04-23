using HomeRunTracker.Core.Interfaces;
using HomeRunTracker.Core.Models;
using HomeRunTracker.Core.Models.Details;
using HomeRunTracker.SharedKernel.Enums;
using HomeRunTracker.SharedKernel.Extensions;

namespace HomeRunTracker.Infrastructure.PitcherGameScore.Services;

public class PitcherGameScoreService : IPitcherGameScoreService
{
    public HashSet<GameScoreRecordDto> GetPitcherGameScores(GameDetailsDto gameDetails)
    {
        var pitcherGameScores = new HashSet<GameScoreRecordDto>();
        
        var playGroupings = gameDetails.Plays
            .GroupBy(x => x.Pitcher);

        foreach (var grouping in playGroupings)
        {
            var pitcher = grouping.Key;
            var plays = grouping.ToList();

            var outs = plays
                .Sum(x => x.HasOut ? 1 : 0);
            var numHits = plays
                .Sum(x => x.Result.IsHit() ? 1 : 0);
            var numStrikeOuts = plays
                .Sum(x => x.Result is EPlayResult.Strikeout ? 1 : 0);
            var earnedRuns = plays
                .Select(x => x.Runners
                    .Sum(y => y is {IsScoringEvent: true, IsEarned: true} ? 1 : 0)).Sum();
            var unearnedRuns = plays
                .Select(x => x.Runners
                    .Sum(y => y is {IsScoringEvent: true, IsEarned: false} ? 1 : 0)).Sum();
            var walks = plays
                .Sum(x => x.Result is EPlayResult.Walk ? 1 : 0);
            
            var (batterTeamId, pitcherTeamId, batterTeamName, pitcherTeamName, _) =
                new PlayTeams(plays[0], gameDetails);
            
            var gameScore = new GameScoreRecordDto
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
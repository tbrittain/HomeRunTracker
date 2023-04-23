using HomeRunTracker.Core.Models.Details;
using HomeRunTracker.Infrastructure.MlbApiService.Models.Details;

namespace HomeRunTracker.Infrastructure.MlbApiService.Mappings;

public static class MlbGameDetailsMapping
{
    public static GameDetailsDto MapToGameDetailsDto(this MlbGameDetails mlbGameDetails)
    {
        return new GameDetailsDto
        {
            Id = mlbGameDetails.Id,
            GameStartTime = mlbGameDetails.GameData.GameDateTime.DateTimeOffset,
            Status = mlbGameDetails.GameData.Status.Status,
            HomeTeam = new TeamDto
            {
                Id = mlbGameDetails.GameData.TeamMatchup.HomeTeam.Id,
                Name = mlbGameDetails.GameData.TeamMatchup.HomeTeam.Name
            },
            AwayTeam = new TeamDto
            {
                Id = mlbGameDetails.GameData.TeamMatchup.AwayTeam.Id,
                Name = mlbGameDetails.GameData.TeamMatchup.AwayTeam.Name
            },
            Plays = mlbGameDetails.LiveData.Plays.AllPlays
                .Where(mlbPlay => mlbPlay.Events.Any())
                .Select(mlbPlay =>
                {
                    var (homeScoreStart, awayScoreStart) = mlbPlay.GetScoreStart();

                    var playEvent = mlbPlay.Events.FirstOrDefault(playEvent => playEvent.HitData is not null) ??
                                    mlbPlay.Events.Last();

                    var playGuid = playEvent.PlayId;
                    
                    HitDataDto? hitDataDto = null;
                    if (playEvent.HitData is not null)
                    {
                        hitDataDto = new HitDataDto
                        {
                            LaunchSpeed = playEvent.HitData.LaunchSpeed,
                            LaunchAngle = playEvent.HitData.LaunchAngle,
                            TotalDistance = playEvent.HitData.TotalDistance
                        };
                    }

                    return new PlayDto
                    {
                        PlayEndTime = mlbPlay.DateTimeOffset,
                        Description = mlbPlay.Result.Description,
                        Rbi = mlbPlay.Result.Rbi,
                        Result = mlbPlay.Result.Result,
                        Inning = mlbPlay.About.Inning,
                        IsTopInning = mlbPlay.About.IsTopInning,
                        HasOut = mlbPlay.About.HasOut,
                        HomeScoreBefore = homeScoreStart,
                        AwayScoreBefore = awayScoreStart,
                        HomeScoreAfter = mlbPlay.Result.HomeScore,
                        AwayScoreAfter = mlbPlay.Result.AwayScore,
                        Outs = mlbPlay.Count.Outs,
                        Batter = new PlayerDto
                        {
                            Id = mlbPlay.PlayerMatchup.Batter.Id,
                            FullName = mlbPlay.PlayerMatchup.Batter.FullName
                        },
                        Pitcher = new PlayerDto
                        {
                            Id = mlbPlay.PlayerMatchup.Pitcher.Id,
                            FullName = mlbPlay.PlayerMatchup.Pitcher.FullName
                        },
                        Runners = mlbPlay.Runners
                            .Select(mlbPlayRunner => new RunnerDto
                            {
                                Base = mlbPlayRunner.Movement.Base,
                                IsScoringEvent = mlbPlayRunner.Details.IsScoringEvent,
                                IsEarned = mlbPlayRunner.Details.Earned,
                                ResponsiblePitcher = mlbPlayRunner.Details.Pitcher is null
                                    ? null
                                    : new PlayerDto
                                    {
                                        Id = mlbPlayRunner.Details.Pitcher.Id,
                                        FullName = mlbPlayRunner.Details.Pitcher.FullName
                                    }
                            })
                            .ToList(),
                        HitData = hitDataDto,
                        Id = playGuid
                    };
                })
                .ToList()
        };
    }
}
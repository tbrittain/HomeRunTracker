using HomeRunTracker.Core.Models.Schedule;
using HomeRunTracker.Infrastructure.MlbApiService.Models.Summary;

namespace HomeRunTracker.Infrastructure.MlbApiService.Mappings;

public static class MlbScheduleMapping
{
    public static ScheduleDto MapToScheduleDto(this MlbSchedule mlbSchedule)
    {
        return new ScheduleDto
        {
            TotalGames = mlbSchedule.TotalGames,
            Games = mlbSchedule.Dates
                .FirstOrDefault()?.Games
                .Select(mlbGameSummary => new GameSummaryDto
                {
                    Id = mlbGameSummary.Id,
                    Status = mlbGameSummary.GameStatus.Status
                })
                .ToList() ?? new List<GameSummaryDto>()
        };
    }
}
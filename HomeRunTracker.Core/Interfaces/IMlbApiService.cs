using System.Net;
using HomeRunTracker.Core.Models.Content;
using HomeRunTracker.Core.Models.Details;
using HomeRunTracker.Core.Models.Schedule;
using OneOf.Types;
using OneOf;

namespace HomeRunTracker.Core.Interfaces;

public interface IMlbApiService
{
    Task<OneOf<ScheduleDto, HttpStatusCode, Error<string>>> FetchGames(DateTime dateTime);
    Task<OneOf<GameDetailsDto, HttpStatusCode, Error<string>>> FetchGameDetails(int gameId);
    Task<OneOf<GameContentDto, HttpStatusCode, Error<string>>> FetchGameContent(int gameId);
}
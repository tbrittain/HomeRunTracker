using System.Net;
using HomeRunTracker.Common.Models.Content;
using HomeRunTracker.Common.Models.Details;
using HomeRunTracker.Common.Models.Summary;
using OneOf;
using OneOf.Types;

namespace HomeRunTracker.Backend.Services.HttpService;

public interface IHttpService
{
    Task<OneOf<MlbSchedule, HttpStatusCode, Error<string>>> FetchGames(DateTime dateTime);
    Task<OneOf<MlbGameDetails, HttpStatusCode, Error<string>>> FetchGameDetails(int gameId);
    Task<OneOf<MlbGameContent, HttpStatusCode, Error<string>>> FetchGameContent(int gameId);
}
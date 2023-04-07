using HomeRunTracker.Common.Models.Details;
using HomeRunTracker.Common.Models.Summary;

namespace HomeRunTracker.Backend.Services.HttpService;

public interface IHttpService
{
    Task<MlbSchedule> FetchGames(DateTime dateTime);
    Task<MlbGameDetails> FetchGameDetails(int gameId);
}
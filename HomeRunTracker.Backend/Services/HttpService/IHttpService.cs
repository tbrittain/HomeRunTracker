using HomeRunTracker.Common.Models.Details;
using HomeRunTracker.Common.Models.Summary;

namespace HomeRunTracker.Backend.Services.HttpService;

public interface IHttpService
{
    Task<MlbSchedule> FetchGamesAsync(DateTime dateTime);
    Task<MlbGameDetails> FetchGameDetailsAsync(int gameId);
}
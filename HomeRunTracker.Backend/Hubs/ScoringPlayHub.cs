using HomeRunTracker.Common.Models.Notifications;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace HomeRunTracker.Backend.Hubs;

public class ScoringPlayHub : Hub
{
    public async Task PublishScoringPlayAsync(ScoringPlayNotification scoringPlayNotification)
    {
        await Clients.All.SendAsync("ReceiveScoringPlay", JsonConvert.SerializeObject(scoringPlayNotification));
    }
}
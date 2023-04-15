using HomeRunTracker.Common.Models.Notifications;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace HomeRunTracker.Backend.Hubs;

public class HomeRunHub : Hub
{
    public async Task PublishHomeRunAsync(HomeRunNotification homeRunNotification)
    {
        await Clients.All.SendAsync("ReceiveHomeRun", JsonConvert.SerializeObject(homeRunNotification));
    }
}
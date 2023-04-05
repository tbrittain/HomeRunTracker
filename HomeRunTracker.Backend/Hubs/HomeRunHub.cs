using HomeRunTracker.Common.Models.Internal;
using Microsoft.AspNetCore.SignalR;

namespace HomeRunTracker.Backend.Hubs;

public class HomeRunHub : Hub
{
    public async Task PublishHomeRunAsync(HomeRunRecord homeRun)
    {
        await Clients.All.SendAsync("ReceiveHomeRun", homeRun);
    }
}
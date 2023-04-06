using HomeRunTracker.Common.Models.Internal;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace HomeRunTracker.Backend.Hubs;

public class HomeRunHub : Hub
{
    public async Task PublishHomeRunAsync(HomeRunRecord homeRun)
    {
        await Clients.All.SendAsync("ReceiveHomeRun", JsonConvert.SerializeObject(homeRun));
    }
}
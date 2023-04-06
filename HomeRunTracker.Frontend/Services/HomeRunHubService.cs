using HomeRunTracker.Common.Models.Internal;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace HomeRunTracker.Frontend.Services;

public class HomeRunHubService
{
    private readonly HubConnection _hubConnection;

    public HomeRunHubService()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5001/home-run-hub")
            .WithAutomaticReconnect()
            .Build();
    }

    public async Task StartHubConnection()
    {
        await _hubConnection.StartAsync();
    }

    public void SubscribeToHubMethods()
    {
        _hubConnection.On<string>("ReceiveHomeRun", json =>
        {
            var homeRun = JsonConvert.DeserializeObject<HomeRunRecord>(json);
            if (homeRun is null)
            {
                throw new InvalidOperationException("Home run is null");
            }

            OnHomeRunReceived?.Invoke(homeRun);
        });
    }
    
    public event Func<HomeRunRecord, Task>? OnHomeRunReceived;
}
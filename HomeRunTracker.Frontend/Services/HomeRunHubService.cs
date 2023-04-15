using HomeRunTracker.Common.Models.Notifications;
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
            var homeRun = JsonConvert.DeserializeObject<HomeRunNotification>(json);
            if (homeRun is null)
            {
                throw new InvalidOperationException("Home run is null");
            }

            OnHomeRunReceived?.Invoke(homeRun);
        });

        _hubConnection.On<string>("UpdateHomeRun", json =>
        {
            var notification = JsonConvert.DeserializeObject<HomeRunUpdatedNotification>(json);
            if (notification is null)
            {
                throw new InvalidOperationException("Home run update notification is null");
            }
            
            OnHomeRunUpdated?.Invoke(notification);
        });
    }

    public event Func<HomeRunNotification, Task>? OnHomeRunReceived;
    
    public event Func<HomeRunUpdatedNotification, Task>? OnHomeRunUpdated;
}
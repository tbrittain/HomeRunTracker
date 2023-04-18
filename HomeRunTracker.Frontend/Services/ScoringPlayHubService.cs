using HomeRunTracker.Common.Models.Notifications;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace HomeRunTracker.Frontend.Services;

public class ScoringPlayHubService
{
    private readonly HubConnection _hubConnection;

    public ScoringPlayHubService()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5001/scoring-play-hub")
            .WithAutomaticReconnect()
            .Build();
    }

    public async Task StartHubConnection()
    {
        await _hubConnection.StartAsync();
    }

    public void SubscribeToHubMethods()
    {
        _hubConnection.On<string>("ReceiveScoringPlay", json =>
        {
            var homeRun = JsonConvert.DeserializeObject<ScoringPlayNotification>(json);
            if (homeRun is null)
            {
                throw new InvalidOperationException("Home run is null");
            }

            OnScoringPlayReceived?.Invoke(homeRun);
        });

        _hubConnection.On<string>("UpdateScoringPlay", json =>
        {
            var notification = JsonConvert.DeserializeObject<ScoringPlayUpdatedNotification>(json);
            if (notification is null)
            {
                throw new InvalidOperationException("Home run update notification is null");
            }
            
            OnScoringPlayUpdated?.Invoke(notification);
        });
    }

    public event Func<ScoringPlayNotification, Task>? OnScoringPlayReceived;
    
    public event Func<ScoringPlayUpdatedNotification, Task>? OnScoringPlayUpdated;
}
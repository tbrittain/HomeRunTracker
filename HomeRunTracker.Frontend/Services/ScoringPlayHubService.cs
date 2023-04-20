using HomeRunTracker.Common.Models.Notifications;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace HomeRunTracker.Frontend.Services;

public class ScoringPlayHubService
{
    private bool _isHubConnected;
    private bool _isHubSubscribed;
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
        if (_isHubConnected) return;

        await _hubConnection.StartAsync();

        _isHubConnected = true;
    }

    public void SubscribeToHubMethods()
    {
        if (_isHubSubscribed) return;

        _hubConnection.On<string>("ReceiveScoringPlay", json =>
        {
            var homeRun = JsonConvert.DeserializeObject<ScoringPlayNotification>(json);
            if (homeRun is null)
            {
                throw new InvalidOperationException("Scoring play is null");
            }

            OnScoringPlayReceived?.Invoke(homeRun);
        });

        _hubConnection.On<string>("UpdateScoringPlay", json =>
        {
            var notification = JsonConvert.DeserializeObject<ScoringPlayUpdatedNotification>(json);
            if (notification is null)
            {
                throw new InvalidOperationException("Scoring play update notification is null");
            }

            OnScoringPlayUpdated?.Invoke(notification);
        });

        _hubConnection.On<string>("ReceiveGameScore", json =>
        {
            var notification = JsonConvert.DeserializeObject<GameScoreNotification>(json);
            if (notification is null)
            {
                throw new InvalidOperationException("Game score notification is null");
            }

            OnGameScoreReceived?.Invoke(notification);
        });
        
        _isHubSubscribed = true;
    }

    public event Func<ScoringPlayNotification, Task>? OnScoringPlayReceived;

    public event Func<ScoringPlayUpdatedNotification, Task>? OnScoringPlayUpdated;
    
    public event Func<GameScoreNotification, Task>? OnGameScoreReceived;
}
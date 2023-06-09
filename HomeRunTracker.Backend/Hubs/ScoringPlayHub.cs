﻿using HomeRunTracker.Backend.Actions.GameScore.Notifications;
using HomeRunTracker.Backend.Actions.ScoringPlay.Notifications;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace HomeRunTracker.Backend.Hubs;

public class ScoringPlayHub : Hub
{
    public async Task PublishScoringPlayAsync(ScoringPlayNotification scoringPlayNotification)
    {
        await Clients.All.SendAsync("ReceiveScoringPlay", JsonConvert.SerializeObject(scoringPlayNotification));
    }

    public async Task PublishGameScoreNotification(GameScoreNotification gameScoreNotification)
    {
        await Clients.All.SendAsync("ReceiveGameScore", JsonConvert.SerializeObject(gameScoreNotification));
    }
}
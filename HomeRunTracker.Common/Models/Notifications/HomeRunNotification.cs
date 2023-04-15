﻿using HomeRunTracker.Common.Models.Internal;
using MediatR;

namespace HomeRunTracker.Common.Models.Notifications;

[GenerateSerializer]
public class HomeRunNotification : INotification
{
    public HomeRunNotification(int gameId, DateTime gameStartTime, HomeRunRecord homeRun)
    {
        GameId = gameId;
        GameStartTime = gameStartTime;
        HomeRun = homeRun;
    }

    [Id(0)]
    public int GameId { get; }
    
    [Id(1)]
    public DateTime GameStartTime { get; }

    [Id(2)]
    public HomeRunRecord HomeRun { get; }
}
﻿using HomeRunTracker.Backend.Actions.GameScore.Notifications;
using HomeRunTracker.Backend.Grains;
using MediatR;

namespace HomeRunTracker.Backend.Actions.GameScore.Handlers;

public class GameScoreHandler : INotificationHandler<GameScoreNotification>
{
    private readonly IClusterClient _clusterClient;

    public GameScoreHandler(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public async Task Handle(GameScoreNotification notification, CancellationToken cancellationToken)
    {
        var gameListGrain = _clusterClient.GetGrain<IGameListGrain>(0);
        await gameListGrain.PublishGameScore(notification);
    }
}
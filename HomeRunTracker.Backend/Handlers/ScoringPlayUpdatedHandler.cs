using HomeRunTracker.Backend.Grains;
using HomeRunTracker.Core.Actions.ScoringPlays.Notifications;
using MediatR;

namespace HomeRunTracker.Backend.Handlers;

public class ScoringPlayUpdatedHandler : INotificationHandler<ScoringPlayUpdatedNotification>
{
    private readonly IClusterClient _clusterClient;

    public ScoringPlayUpdatedHandler(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public Task Handle(ScoringPlayUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var gameListGrain = _clusterClient.GetGrain<IGameListGrain>(0);
        return gameListGrain.PublishScoringPlayUpdated(notification);
    }
}
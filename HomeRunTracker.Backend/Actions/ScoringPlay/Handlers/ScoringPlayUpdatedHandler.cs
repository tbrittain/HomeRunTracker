using HomeRunTracker.Backend.Actions.ScoringPlay.Notifications;
using HomeRunTracker.Backend.Grains;
using MediatR;

namespace HomeRunTracker.Backend.Actions.ScoringPlay.Handlers;

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
using HomeRunTracker.Backend.Actions.ScoringPlay.Notifications;
using HomeRunTracker.Backend.Grains;
using MediatR;

namespace HomeRunTracker.Backend.Actions.ScoringPlay.Handlers;

public class HomeRunHandler : INotificationHandler<ScoringPlayNotification>
{
    private readonly IClusterClient _clusterClient;

    public HomeRunHandler(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public async Task Handle(ScoringPlayNotification notification, CancellationToken cancellationToken)
    {
        var gameListGrain = _clusterClient.GetGrain<IGameListGrain>(0);
        await gameListGrain.PublishScoringPlay(notification);
    }
}
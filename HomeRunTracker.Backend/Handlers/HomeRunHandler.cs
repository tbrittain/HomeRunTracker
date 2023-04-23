using HomeRunTracker.Backend.Grains;
using HomeRunTracker.Backend.Models.Notifications;
using MediatR;

namespace HomeRunTracker.Backend.Handlers;

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
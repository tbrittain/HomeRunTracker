using HomeRunTracker.Backend.Grains;
using HomeRunTracker.Common.Models.Notifications;
using MediatR;

namespace HomeRunTracker.Backend.Handlers;

public class HomeRunUpdatedHandler : INotificationHandler<HomeRunUpdatedNotification>
{
    private readonly IClusterClient _clusterClient;

    public HomeRunUpdatedHandler(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public Task Handle(HomeRunUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var gameListGrain = _clusterClient.GetGrain<IGameListGrain>(0);
        return gameListGrain.PublishHomeRunUpdated(notification);
    }
}
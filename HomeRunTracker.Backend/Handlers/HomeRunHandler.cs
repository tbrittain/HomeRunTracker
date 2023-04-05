using HomeRunTracker.Backend.Grains;
using HomeRunTracker.Common.Models.Notifications;
using MediatR;

namespace HomeRunTracker.Backend.Handlers;

public class HomeRunHandler : INotificationHandler<HomeRunNotification>
{
    private readonly IClusterClient _clusterClient;

    public HomeRunHandler(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public async Task Handle(HomeRunNotification notification, CancellationToken cancellationToken)
    {
        var gameListGrain = _clusterClient.GetGrain<IGameListGrain>(0);
        await gameListGrain.PublishHomeRunAsync(notification);
    }
}
using HomeRunTracker.Common.Models.Notifications;
using MediatR;

namespace HomeRunTracker.Backend.Handlers;

public class HomeRunHandler : INotificationHandler<HomeRunNotification>
{
    private readonly IServiceProvider _serviceProvider;
    
    public HomeRunHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Handle(HomeRunNotification notification, CancellationToken cancellationToken)
    {
        // TODO: this
        await Task.Delay(10, cancellationToken);
    }
}
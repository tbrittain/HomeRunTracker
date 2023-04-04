using HomeRunTracker.Backend.Services;
using HomeRunTracker.Common.Models.Notifications;
using MediatR;

namespace HomeRunTracker.Backend.Handlers;

public class GameRemovedHandler : INotificationHandler<GameStoppedNotification>
{
    private readonly IServiceProvider _serviceProvider;
    
    public GameRemovedHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task Handle(GameStoppedNotification notification, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var pollingService = scope.ServiceProvider.GetService<MlbApiPollingService>();
        if (pollingService is null)
            throw new InvalidOperationException("MlbApiPollingService not found");
        
        pollingService.RemoveGame(notification.GameId);
        return Task.CompletedTask;
    }
}
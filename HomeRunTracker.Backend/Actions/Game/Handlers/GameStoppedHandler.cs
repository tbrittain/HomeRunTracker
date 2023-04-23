using HomeRunTracker.Backend.Actions.Game.Notifications;
using HomeRunTracker.Backend.Services;
using MediatR;

namespace HomeRunTracker.Backend.Actions.Game.Handlers;

public class GameStoppedHandler : INotificationHandler<GameStoppedNotification>
{
    private readonly IServiceProvider _serviceProvider;
    
    public GameStoppedHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task Handle(GameStoppedNotification notification, CancellationToken cancellationToken)
    {
        var pollingService = _serviceProvider.GetService<MlbCurrentDayGamePollingService>();
        if (pollingService is null)
            throw new InvalidOperationException($"{nameof(MlbCurrentDayGamePollingService)} not found");
        
        pollingService.UntrackGame(notification.GameId);
        return Task.CompletedTask;
    }
}
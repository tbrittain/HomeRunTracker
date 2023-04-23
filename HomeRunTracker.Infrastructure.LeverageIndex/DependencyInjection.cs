using HomeRunTracker.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeRunTracker.Infrastructure.LeverageIndex;

public static class DependencyInjection
{
    public static IServiceCollection AddLeverageIndexService(this IServiceCollection services)
    {
        services.AddSingleton<ILeverageIndexService, Services.LeverageIndexService>();
        return services;
    }
}
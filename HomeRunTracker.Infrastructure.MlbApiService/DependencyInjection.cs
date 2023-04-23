using HomeRunTracker.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeRunTracker.Infrastructure.MlbApiService;

public static class DependencyInjection
{
    public static IServiceCollection AddMlbApiService(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<IMlbApiService, Services.MlbApiService>();

        return services;
    }
}
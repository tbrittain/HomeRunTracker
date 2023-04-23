using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace HomeRunTracker.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetCallingAssembly()));
        
        return services;
    }
}
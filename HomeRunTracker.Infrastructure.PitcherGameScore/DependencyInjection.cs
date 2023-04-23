using HomeRunTracker.Core.Interfaces;
using HomeRunTracker.Infrastructure.PitcherGameScore.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HomeRunTracker.Infrastructure.PitcherGameScore;

public static class DependencyInjection
{
    public static IServiceCollection AddPitcherGameScoreService(this IServiceCollection services)
    {
        services.AddSingleton<IPitcherGameScoreService, PitcherGameScoreService>();

        return services;
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenisu.Application.Interfaces;
using Tenisu.Application.Services;


namespace Tenisu.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.TryAddSingleton<IPlayerService, PlayerService>();
            services.TryAddSingleton<IStatisticsService, StatisticsService>();
            return services;
        }
    }
}

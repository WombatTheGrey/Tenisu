using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenisu.Application.DTOs;
using Tenisu.Application.Interfaces;
using Tenisu.Application.Services;


namespace Tenisu.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.TryAddScoped<IPlayerService, PlayerService>();
            services.TryAddScoped<IStatisticsService, StatisticsService>();
            services.TryAddSingleton<TenisuMapper>();
            return services;
        }
    }
}

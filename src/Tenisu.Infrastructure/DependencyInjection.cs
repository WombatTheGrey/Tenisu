using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tenisu.Domain.Interfaces;
using Tenisu.Infrastructure.Context;
using Tenisu.Infrastructure.Initialization;
using Tenisu.Infrastructure.Repositories;

namespace Tenisu.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("TenisuDB")
                ?? throw new InvalidOperationException("Connection string 'TenisuDB' not found.");

            services.AddDbContextPool<TenisuDbContext>(options =>
            {
                options.UseSqlServer(connectionString);

                options.UseSeeding((db, _) =>
                {
                    TenisuSeeder.SeedAsync((TenisuDbContext)db, CancellationToken.None)
                    .GetAwaiter().GetResult();
                });

                options.UseAsyncSeeding(async (db, _, cancellationToken) =>
                {
                    await TenisuSeeder.SeedAsync((TenisuDbContext)db, cancellationToken);
                });
            });

            services.TryAddScoped<ITenisuRepository, TenisuRepository>();
            services.TryAddScoped<IStatisticsRepository, StatisticsRepository>();
            services.TryAddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}

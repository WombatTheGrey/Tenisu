using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tenisu.Infrastructure.Context;

namespace Tenisu.Infrastructure.Initialization
{
    public static class TenisuDBInitializer
    {
        public static async ValueTask InitializeAsync(IServiceProvider serviceProvider, CancellationToken token)
        {
            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<TenisuDbContext>();

            await dbContext.Database.MigrateAsync(token);
        }
    }
}

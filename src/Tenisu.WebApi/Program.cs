using Microsoft.EntityFrameworkCore;
using Tenisu.Infrastructure;
using Tenisu.Infrastructure.Context;
using Tenisu.Infrastructure.Initialization;

namespace Tenisu.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();

            if(app.Configuration.GetValue<bool>("EnableDatabaseMigration"))
            {
                await TenisuDBInitializer.InitializeAsync(app.Services, app.Lifetime.ApplicationStopping);
            }

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}

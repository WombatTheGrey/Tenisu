using System.Text.Json.Serialization;
using Tenisu.Application;
using Tenisu.Infrastructure;
using Tenisu.Infrastructure.Initialization;
using Tenisu.WebApi.Handlers;

namespace Tenisu.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //Builder :
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();

            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            builder.Services.AddOpenApi();
            builder.Services.ConfigureRateLimiting(builder.Configuration);
            
            //App :
            var app = builder.Build();
            try
            {
                if (app.Configuration.GetValue<bool>("EnableDatabaseMigration"))
                {
                    await TenisuDBInitializer.InitializeAsync(app.Services, app.Lifetime.ApplicationStopping);
                }

                app.UseExceptionHandler();

                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "Tenisu API");
                    options.DocumentTitle = "Tenisu API";
                });

                app.UseHttpsRedirection();
                app.UseRateLimiter();
                app.UseAuthorization();
                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogCritical(ex, "Application terminated unexpectedly.");
                throw;
            }
        }
    }
}

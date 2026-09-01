using System.Text.Json.Serialization;
using Serilog;
using Tenisu.Application;
using Tenisu.Infrastructure;
using Tenisu.Infrastructure.Context;
using Tenisu.Infrastructure.Initialization;
using Tenisu.WebApi.Handlers;

namespace Tenisu.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            Log.Information("Tenisu API is starting.");
            try
            {
                //Builder :
                var builder = WebApplication.CreateBuilder(args);
                builder.Configuration.ConfigureAzureKeyVault();

                builder.Host.ConfigureSerilog();

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

                builder.Services.AddHealthChecks()
                    .AddDbContextCheck<TenisuDbContext>("TenisuDB");

                builder.Services.AddOpenApi();
                builder.Services.ConfigureRateLimiting(builder.Configuration);

                //App :
                var app = builder.Build();
                app.UseSerilogRequestLogging();
                app.UseExceptionHandler();                

                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "Tenisu API");
                    options.DocumentTitle = "Tenisu API";
                });

                app.UseHttpsRedirection();
                app.UseRateLimiter();
                app.MapControllers();
                app.MapHealthChecks("/health");

                if (app.Configuration.GetValue<bool>("EnableDatabaseMigration"))
                {
                    await TenisuDBInitializer.InitializeAsync(app.Services, app.Lifetime.ApplicationStopping);
                }

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly.");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}

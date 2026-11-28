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

            Log.Information("Tenisu API v:{version} is starting", AppVersion.Value);
            try
            {
                //Builder :
                var builder = WebApplication.CreateBuilder(args);
                Log.Information("Tenisu API v:{version} is starting : Building", AppVersion.Value);
                builder.Configuration.ConfigureAzureKeyVault();

                builder.ConfigureLogging();

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

                builder.Services.AddOpenApi(options =>
                {
                    options.AddDocumentTransformer((document, context, cancellationToken) =>
                    {
                        document.Info.Version = AppVersion.Value;
                        return Task.CompletedTask;
                    });
                });
                builder.Services.ConfigureRateLimiting(builder.Configuration);


                //App :
                Log.Information("Tenisu API v:{version} is starting : Preparing the run", AppVersion.Value);
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

                Log.Information("Tenisu API v:{version is starting : Running", AppVersion.Value);
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

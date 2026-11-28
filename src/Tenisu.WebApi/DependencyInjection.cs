using System.Threading.RateLimiting;
using Azure.Identity;
using Serilog;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;

namespace Tenisu.WebApi
{
    public static class DependencyInjection
    {
        public static void ConfigureAzureKeyVault(this ConfigurationManager configuration)
        {
            var keyVaultUri = configuration.GetValue<string>("KeyVaultUri");
            if (string.IsNullOrWhiteSpace(keyVaultUri))
            {
                Log.Warning("No KeyVaultUri defined, the Azure key vault will be skipped");
                return;
            }
            configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
        }

        public static void ConfigureRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetRequiredSection("RateLimiting");
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(_ =>
                    RateLimitPartition.GetFixedWindowLimiter("global", _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = section.GetValue<int>("PermitLimit"),
                        Window = TimeSpan.FromMinutes(section.GetValue<int>("Window"))
                    }));
            });
        }

        public static void ConfigureLogging(this WebApplicationBuilder builder)
        {
            Serilog.Debugging.SelfLog.Enable(msg =>
            {
                Console.Error.WriteLine("SERILOG: " + msg);
            });

            var connectionString = builder.Configuration.GetConnectionString("ApplicationInsight");
            if(connectionString == null)
            {
                Log.Warning("No ApplicationInsight connection string defined. Logging to console Only !");
                return;
            }

            //used to log dependencies and requests in appInsight. Serilog is used for traces.
            builder.Services.AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = connectionString;
            });

            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithProperty("version", AppVersion.Value)
                    .WriteTo.Console()
                    .WriteTo.ApplicationInsights(connectionString, new TraceTelemetryConverter());
            });
        }
    }
}

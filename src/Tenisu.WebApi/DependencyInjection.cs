using System.Threading.RateLimiting;
using Azure.Identity;
using Serilog;

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

        public static void ConfigureSerilog(this ConfigureHostBuilder host)
        {
            //secret name = Serilog--WriteTo--appInsights--Args--connectionString
            

            Serilog.Debugging.SelfLog.Enable(msg =>
            {
                Console.Error.WriteLine("SERILOG: " + msg);
            });

            host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();
            });
        }
    }
}

using System.Threading.RateLimiting;

namespace Tenisu.WebApi
{
    public static class DependencyInjection
    {
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

        public static void ConfigureSerilog(this IServiceCollection services)
        {
        }
}

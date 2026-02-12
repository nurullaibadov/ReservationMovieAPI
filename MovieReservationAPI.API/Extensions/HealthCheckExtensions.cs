using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MovieReservationAPI.API.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddHealthChecks(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddSqlServer(
                    configuration.GetConnectionString("DefaultConnection")!,
                    name: "sql-server",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "database", "sql" })
                .AddCheck("memory", () =>
                {
                    var allocatedMb = GC.GetTotalMemory(false) / 1024 / 1024;
                    return allocatedMb < 512
                        ? HealthCheckResult.Healthy(
                            $"Memory: {allocatedMb}MB")
                        : HealthCheckResult.Degraded(
                            $"High memory: {allocatedMb}MB");
                }, tags: new[] { "memory" });

            return services;
        }
    }

}

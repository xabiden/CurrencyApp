using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Extensions;

public static class HealthChecksExtensions
{
    public static IServiceCollection AddPostgresHealthCheck(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName)
    {
        var connectionString =
            configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{connectionStringName}' is not configured.");

        services.AddHealthChecks()
            .AddNpgSql(connectionString);

        return services;
    }
}
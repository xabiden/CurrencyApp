using FinanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UserService.Infrastructure.Persistence;

var builder = Host.CreateApplicationBuilder(args);

var userConnectionString = builder.Configuration.GetConnectionString("UserDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'UserDatabase' is not configured.");

var financeConnectionString = builder.Configuration.GetConnectionString("FinanceDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'FinanceDatabase' is not configured.");

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(userConnectionString));

builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseNpgsql(financeConnectionString));

using var host = builder.Build();
using var scope = host.Services.CreateScope();

var logger = scope.ServiceProvider
    .GetRequiredService<ILoggerFactory>()
    .CreateLogger("MigrationService");

logger.LogInformation("Starting database migrations.");

try
{
    var userDbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    await MigrateAsync(userDbContext, logger);

    var financeDbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
    await MigrateAsync(financeDbContext, logger);

    logger.LogInformation("All migrations completed successfully.");
}
catch (Exception exception)
{
    logger.LogError(exception, "Database migration failed.");
    Environment.Exit(1);
}

static async Task MigrateAsync<TDbContext>(TDbContext dbContext, ILogger logger)
    where TDbContext : DbContext
{
    var name = typeof(TDbContext).Name;
    logger.LogInformation("Applying migrations for {DbContext}.", name);
    await dbContext.Database.MigrateAsync();
    logger.LogInformation("Migrations completed for {DbContext}.", name);
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FinanceService.Infrastructure.Persistence;

public sealed class FinanceDbContextFactory : IDesignTimeDbContextFactory<FinanceDbContext>
{
    public FinanceDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("FINANCE_DATABASE_CONNECTION") ??
            "Host=localhost;Port=5432;Database=currencyapp_finance;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new FinanceDbContext(optionsBuilder.Options);
    }
}

using FinanceService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Persistence;

public sealed class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<UserFavoriteCurrency> UserFavorites => Set<UserFavoriteCurrency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceDbContext).Assembly);
    }
}

using FinanceService.Application.Abstractions.Persistence;

namespace FinanceService.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly FinanceDbContext _dbContext;

    public UnitOfWork(FinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

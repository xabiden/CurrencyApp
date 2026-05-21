using UserService.Application.Abstractions.Persistence;

namespace UserService.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly UserDbContext _dbContext;

    public UnitOfWork(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
using FinanceService.Application.Abstractions.Persistence;
using FinanceService.Domain.Entities;
using FinanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Persistence.Repositories;

public sealed class CurrencyRepository : ICurrencyRepository
{
    private readonly FinanceDbContext _dbContext;

    public CurrencyRepository(FinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Currency?> FindByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await _dbContext.Currencies
            .FirstOrDefaultAsync(c => c.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task<Currency?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Currencies
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsFavoriteAsync(Guid userId, Guid currencyId, CancellationToken cancellationToken)
    {
        return await _dbContext.UserFavorites
            .AsNoTracking()
            .AnyAsync(f => f.UserId == userId && f.CurrencyId == currencyId, cancellationToken);
    }

    public async Task AddFavoriteAsync(UserFavoriteCurrency favorite, CancellationToken cancellationToken)
    {
        _dbContext.UserFavorites.Add(favorite);
        await Task.CompletedTask;
    }
}

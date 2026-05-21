using FinanceService.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Finance;

namespace FinanceService.Infrastructure.Persistence.Repositories;

public sealed class CurrencyReadRepository : ICurrencyReadRepository
{
    private readonly FinanceDbContext _dbContext;

    public CurrencyReadRepository(FinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<UserCurrencyDto>> GetAllCurrenciesAsync(CancellationToken cancellationToken)
    {
        var currencies = await _dbContext.Currencies
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return currencies
            .Select(c => new UserCurrencyDto(c.Id, c.Code, c.Name, c.Rate, false))
            .OrderBy(c => c.Code)
            .ToList();
    }

    public async Task<IReadOnlyCollection<UserCurrencyDto>> GetUserCurrenciesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var favorites = await _dbContext.UserFavorites
            .AsNoTracking()
            .Where(f => f.UserId == userId)
            .Include(f => f.Currency)
            .ToListAsync(cancellationToken);

        return favorites
            .Select(f => new UserCurrencyDto(
                f.Currency.Id,
                f.Currency.Code,
                f.Currency.Name,
                f.Currency.Rate,
                true))
            .OrderBy(c => c.Code)
            .ToList();
    }
}
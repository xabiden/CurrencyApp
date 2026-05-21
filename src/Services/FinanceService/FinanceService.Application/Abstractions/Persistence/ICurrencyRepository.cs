using FinanceService.Domain.Entities;
using Shared.Contracts.Finance;

namespace FinanceService.Application.Abstractions.Persistence;

public interface ICurrencyRepository
{
    Task<Currency?> FindByCodeAsync(string code, CancellationToken cancellationToken);
    Task<Currency?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsAsFavoriteAsync(Guid userId, Guid currencyId, CancellationToken cancellationToken);
    Task AddFavoriteAsync(UserFavoriteCurrency favorite, CancellationToken cancellationToken);
}

using Shared.Contracts.Finance;

namespace FinanceService.Application.Abstractions.Persistence
{
    public interface ICurrencyReadRepository
    {
        Task<IReadOnlyCollection<UserCurrencyDto>> GetAllCurrenciesAsync(CancellationToken cancellationToken);
        Task<IReadOnlyCollection<UserCurrencyDto>> GetUserCurrenciesAsync(Guid userId, CancellationToken cancellationToken);
    }
}

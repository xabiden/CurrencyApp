using FinanceService.Application.Abstractions.Persistence;
using FinanceService.Domain.Entities;
using Shared.Authentication;
using Shared.Contracts.Finance;
using Shared.Results;

namespace FinanceService.Application.Currencies.Commands.AddFavoriteCurrency
{
    public abstract class AddFavoriteCurrencyHandlerBase
    {
        private readonly ICurrencyRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserContext _currentUserContext;

        protected AddFavoriteCurrencyHandlerBase(
            ICurrencyRepository repository,
            IUnitOfWork unitOfWork,
            ICurrentUserContext currentUserContext)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUserContext = currentUserContext;
        }

        protected async Task<Result<UserCurrencyDto>> HandleAsync(
            Currency? currency,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserContext.UserId
                ?? throw new InvalidOperationException(
                    "UserId is null on an authorized endpoint. Check authentication middleware.");

            if (currency is null)
                return Result<UserCurrencyDto>.NotFound("Currencies.NotFound", "Currency not found.");

            var isDuplicate = await _repository.ExistsAsFavoriteAsync(userId, currency.Id, cancellationToken);
            if (isDuplicate)
                return Result<UserCurrencyDto>.Conflict(
                    "Currencies.AlreadyFavorite", "Currency is already in favorites.");

            var favorite = UserFavoriteCurrency.Create(userId, currency.Id);
            await _repository.AddFavoriteAsync(favorite, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<UserCurrencyDto>.Success(new UserCurrencyDto(currency.Id, currency.Code, currency.Name, currency.Rate, true));
        }
    }

}

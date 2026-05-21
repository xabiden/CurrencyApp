using FinanceService.Application.Abstractions.Persistence;
using MediatR;
using Shared.Authentication;
using Shared.Contracts.Finance;
using Shared.Results;

namespace FinanceService.Application.Currencies.Commands.AddFavoriteCurrency;

public sealed record AddFavoriteCurrencyByCodeCommand(string CurrencyCode)
    : IRequest<Result<UserCurrencyDto>>;

public sealed class AddFavoriteCurrencyByCodeCommandHandler
    : AddFavoriteCurrencyHandlerBase,
        IRequestHandler<AddFavoriteCurrencyByCodeCommand, Result<UserCurrencyDto>>
{
    private readonly ICurrencyRepository _repository;

    public AddFavoriteCurrencyByCodeCommandHandler(
        ICurrencyRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserContext currentUserContext)
        : base(repository, unitOfWork, currentUserContext)
    {
        _repository = repository;
    }

    public async Task<Result<UserCurrencyDto>> Handle(
        AddFavoriteCurrencyByCodeCommand request,
        CancellationToken cancellationToken)
    {
        var currency = await _repository.FindByCodeAsync(request.CurrencyCode, cancellationToken);
        return await HandleAsync(currency, cancellationToken);
    }
}

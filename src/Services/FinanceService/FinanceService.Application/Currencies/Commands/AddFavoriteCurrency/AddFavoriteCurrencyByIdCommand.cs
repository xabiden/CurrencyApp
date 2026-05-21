using FinanceService.Application.Abstractions.Persistence;
using MediatR;
using Shared.Authentication;
using Shared.Contracts.Finance;
using Shared.Results;

namespace FinanceService.Application.Currencies.Commands.AddFavoriteCurrency;

public sealed record AddFavoriteCurrencyByIdCommand(Guid CurrencyId)
    : IRequest<Result<UserCurrencyDto>>;

public sealed class AddFavoriteCurrencyByIdCommandHandler
    : AddFavoriteCurrencyHandlerBase,
      IRequestHandler<AddFavoriteCurrencyByIdCommand, Result<UserCurrencyDto>>
{
    private readonly ICurrencyRepository _repository;

    public AddFavoriteCurrencyByIdCommandHandler(
        ICurrencyRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserContext currentUserContext)
        : base(repository, unitOfWork, currentUserContext)
    {
        _repository = repository;
    }

    public async Task<Result<UserCurrencyDto>> Handle(
        AddFavoriteCurrencyByIdCommand request,
        CancellationToken cancellationToken)
    {
        var currency = await _repository.FindByIdAsync(request.CurrencyId, cancellationToken);
        return await HandleAsync(currency, cancellationToken);
    }
}

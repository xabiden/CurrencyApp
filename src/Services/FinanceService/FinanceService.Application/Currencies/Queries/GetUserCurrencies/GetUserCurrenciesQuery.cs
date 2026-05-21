using FinanceService.Application.Abstractions.Persistence;
using MediatR;
using Shared.Authentication;
using Shared.Contracts.Finance;
using Shared.Results;

namespace FinanceService.Application.Currencies.Queries.GetUserCurrencies;

public sealed record GetUserCurrenciesQuery() : IRequest<Result<IReadOnlyCollection<UserCurrencyDto>>>;

public sealed class GetUserCurrenciesQueryHandler : IRequestHandler<GetUserCurrenciesQuery, Result<IReadOnlyCollection<UserCurrencyDto>>>
{
    private readonly ICurrencyReadRepository _readRepository;
    private readonly ICurrentUserContext _currentUserContext;

    public GetUserCurrenciesQueryHandler(
        ICurrencyReadRepository readRepository,
        ICurrentUserContext currentUserContext)
    {
        _readRepository = readRepository;
        _currentUserContext = currentUserContext;
    }

    public async Task<Result<IReadOnlyCollection<UserCurrencyDto>>> Handle(
        GetUserCurrenciesQuery request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUserContext.UserId
            ?? throw new InvalidOperationException(
                "UserId is null on an authorized endpoint. Check authentication middleware.");

        var currencies = await _readRepository.GetUserCurrenciesAsync(userId, cancellationToken);
        return Result<IReadOnlyCollection<UserCurrencyDto>>.Success(currencies);
    }
}

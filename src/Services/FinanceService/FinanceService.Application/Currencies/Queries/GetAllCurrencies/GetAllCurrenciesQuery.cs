using FinanceService.Application.Abstractions.Persistence;
using MediatR;
using Shared.Contracts.Finance;
using Shared.Results;

namespace FinanceService.Application.Currencies.Queries.GetAllCurrencies;

public sealed record GetAllCurrenciesQuery : IRequest<Result<IReadOnlyCollection<UserCurrencyDto>>>;

public sealed class GetAllCurrenciesQueryHandler
    : IRequestHandler<GetAllCurrenciesQuery, Result<IReadOnlyCollection<UserCurrencyDto>>>
{
    private readonly ICurrencyReadRepository _readRepository;

    public GetAllCurrenciesQueryHandler(ICurrencyReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<Result<IReadOnlyCollection<UserCurrencyDto>>> Handle(
        GetAllCurrenciesQuery request,
        CancellationToken cancellationToken)
    {
        var currencies = await _readRepository.GetAllCurrenciesAsync(cancellationToken);
        return Result<IReadOnlyCollection<UserCurrencyDto>>.Success(currencies);
    }
}
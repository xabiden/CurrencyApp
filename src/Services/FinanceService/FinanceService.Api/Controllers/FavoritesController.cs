using FinanceService.Api.Contracts;
using FinanceService.Application.Currencies.Commands.AddFavoriteCurrency;
using FinanceService.Application.Currencies.Queries.GetUserCurrencies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Api.Extensions;
using Shared.Contracts;
using Shared.Contracts.Finance;
using Shared.Results;

namespace FinanceService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class FavoritesController : ControllerBase
{
    private readonly ISender _sender;

    public FavoritesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<UserCurrencyDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFavorites(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUserCurrenciesQuery(), cancellationToken);
        return result.IsFailure
            ? this.ToFailureActionResult(result)
            : Ok(new ApiResponse<IReadOnlyCollection<UserCurrencyDto>>(result.Value));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserCurrencyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddFavorite(
        [FromBody] AddFavoriteCurrencyRequest request,
        CancellationToken cancellationToken)
    {
        Result<UserCurrencyDto> result = request switch
        {
            { CurrencyId: not null } => await _sender.Send(
                new AddFavoriteCurrencyByIdCommand(request.CurrencyId.Value), cancellationToken),

            { CurrencyCode: not null } => await _sender.Send(
                new AddFavoriteCurrencyByCodeCommand(request.CurrencyCode), cancellationToken),

            _ => Result<UserCurrencyDto>.Validation(
                "Currencies.IdentifierRequired",
                "Either currencyCode or currencyId must be provided.")
        };

        return result.IsFailure
            ? this.ToFailureActionResult(result)
            : Ok(new ApiResponse<UserCurrencyDto>(result.Value, "Currency added to favorites."));
    }
}
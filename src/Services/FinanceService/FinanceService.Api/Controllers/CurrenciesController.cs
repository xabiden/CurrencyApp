using FinanceService.Application.Currencies.Queries.GetAllCurrencies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Api.Extensions;
using Shared.Contracts;
using Shared.Contracts.Finance;

namespace FinanceService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class CurrenciesController : ControllerBase
{
    private readonly ISender _sender;

    public CurrenciesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<UserCurrencyDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllCurrenciesQuery(), cancellationToken);
        return result.IsFailure
            ? this.ToFailureActionResult(result)
            : Ok(new ApiResponse<IReadOnlyCollection<UserCurrencyDto>>(result.Value));
    }
}
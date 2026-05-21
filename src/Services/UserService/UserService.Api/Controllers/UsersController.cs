using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Api.Extensions;
using Shared.Contracts;
using Shared.Contracts.Users;
using UserService.Api.Contracts;
using UserService.Application.Users.Commands.LoginUser;
using UserService.Application.Users.Commands.LogoutUser;
using UserService.Application.Users.Commands.RegisterUser;
using UserService.Application.Users.Queries.GetCurrentUser;

namespace UserService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
    [FromBody] RegisterUserRequest request,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new RegisterUserCommand(request.Name, request.Password),
            cancellationToken);

        return result.IsFailure
            ? this.ToFailureActionResult(result)
            : CreatedAtAction(
                nameof(Me),
                new { },
                new ApiResponse<UserDto>(result.Value, "User registered."));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new LoginUserCommand(request.Name, request.Password),
            cancellationToken);

        return result.IsFailure
            ? this.ToFailureActionResult(result)
            : Ok(new ApiResponse<AuthTokenResponse>(result.Value, "Login successful."));
    }

    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new LogoutUserCommand(), cancellationToken);

        return result.IsFailure
            ? this.ToFailureActionResult(result)
            : Ok(new ApiResponse<string>("Logout successful. Remove the token on the client side."));
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCurrentUserQuery(), cancellationToken);

        return result.IsFailure
            ? this.ToFailureActionResult(result)
            : Ok(new ApiResponse<UserDto>(result.Value));
    }
}

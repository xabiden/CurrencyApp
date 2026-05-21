using MediatR;
using Shared.Contracts.Users;
using Shared.Results;
using UserService.Application.Abstractions.Authentication;
using UserService.Application.Abstractions.Persistence;

namespace UserService.Application.Users.Commands.LoginUser;

public sealed record LoginUserCommand(string Name, string Password) : IRequest<Result<AuthTokenResponse>>;

public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<AuthTokenResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthTokenResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim().ToLowerInvariant();

        var user = await _userRepository.GetByNameAsync(name, cancellationToken);

        if (user is null || !_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
        {
            return Result<AuthTokenResponse>.Unauthorized("Users.InvalidCredentials", "Invalid credentials.");
        }

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Name);
        return Result<AuthTokenResponse>.Success(token);
    }
}

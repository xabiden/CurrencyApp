using MediatR;
using Shared.Results;

namespace UserService.Application.Users.Commands.LogoutUser;

public sealed record LogoutUserCommand : IRequest<Result>;

public sealed class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, Result>
{
    public Task<Result> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        => Task.FromResult(Result.Success());
}
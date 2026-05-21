using MediatR;
using Shared.Authentication;
using Shared.Contracts.Users;
using Shared.Results;

namespace UserService.Application.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery : IRequest<Result<UserDto>>;

public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    private readonly ICurrentUserContext _currentUserContext;

    public GetCurrentUserQueryHandler(ICurrentUserContext currentUserContext)
    {
        _currentUserContext = currentUserContext;
    }

    public Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserContext.UserId
            ?? throw new InvalidOperationException(
                "UserId is null on an authorized endpoint. Check authentication middleware.");

        var userName = _currentUserContext.UserName
            ?? throw new InvalidOperationException(
                "UserName is null on an authorized endpoint. Check authentication middleware.");

        return Task.FromResult(Result<UserDto>.Success(new UserDto(userId, userName)));
    }
}

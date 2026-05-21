using Shared.Contracts.Users;

namespace UserService.Application.Abstractions.Authentication;

public interface IJwtTokenGenerator
{
    AuthTokenResponse GenerateToken(Guid userId, string userName);
}

namespace Shared.Contracts.Users;

public sealed record AuthTokenResponse(string Token, DateTime ExpiresAtUtc);

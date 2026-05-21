namespace Shared.Contracts.Finance;

public sealed record UserCurrencyDto(Guid CurrencyId, string Code, string Name, decimal Rate, bool IsFavorite);

namespace FinanceService.Api.Contracts;

public sealed record AddFavoriteCurrencyRequest(string? CurrencyCode, Guid? CurrencyId);

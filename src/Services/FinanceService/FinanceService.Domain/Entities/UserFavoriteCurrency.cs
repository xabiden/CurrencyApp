namespace FinanceService.Domain.Entities;

public class UserFavoriteCurrency
{
    public Guid UserId { get; private set; }
    public Guid CurrencyId { get; private set; }
    public Currency Currency { get; private set; } = null!;

    private UserFavoriteCurrency() { }

    public static UserFavoriteCurrency Create(Guid userId, Guid currencyId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));

        if (currencyId == Guid.Empty)
            throw new ArgumentException("CurrencyId cannot be empty.", nameof(currencyId));

        return new UserFavoriteCurrency
        {
            UserId = userId,
            CurrencyId = currencyId
        };
    }
}

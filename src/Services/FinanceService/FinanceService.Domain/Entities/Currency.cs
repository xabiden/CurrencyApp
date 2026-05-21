namespace FinanceService.Domain.Entities;

public class Currency
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public decimal Rate { get; private set; }

    private Currency()
    {
        Code = string.Empty;
        Name = string.Empty;
    }

    public Currency(Guid id, string code, string name, decimal rate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (rate <= 0)
            throw new ArgumentOutOfRangeException(nameof(rate), "Rate must be positive.");

        Id = id;
        Code = code.ToUpperInvariant();
        Name = name;
        Rate = rate;
    }

    public void Update(string name, decimal rate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (rate <= 0)
            throw new ArgumentOutOfRangeException(nameof(rate), "Rate must be positive.");

        Name = name;
        Rate = rate;
    }
}

using FinanceService.Domain.Entities;
using FinanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackgroundWorker.Services;

public sealed class CurrencyRatesSynchronizer
{
    private readonly ILogger<CurrencyRatesSynchronizer> _logger;
    private readonly CurrencyRatesClient _currencyRatesClient;
    private readonly FinanceDbContext _dbContext;

    public CurrencyRatesSynchronizer(
        ILogger<CurrencyRatesSynchronizer> logger, 
        CurrencyRatesClient currencyRatesClient, 
        FinanceDbContext dbContext)
    {
        _logger = logger;
        _currencyRatesClient = currencyRatesClient;
        _dbContext = dbContext;
    }

    public async Task<int> UpdateRatesAsync(CancellationToken cancellationToken)
    {
        var dailyRates = await _currencyRatesClient.GetDailyRatesAsync(cancellationToken);

        if (dailyRates.Count == 0)
        {
            _logger.LogWarning("No currency rates received from CBR. Database update skipped.");
            return 0;
        }

        var dailyRatesByCode = dailyRates
            .DistinctBy(rate => rate.Code)
            .ToList();

        var existingCurrencies = await _dbContext.Currencies.ToDictionaryAsync(currency => currency.Code, cancellationToken);

        foreach (var dailyRate in dailyRatesByCode)
        {
            if (existingCurrencies.TryGetValue(dailyRate.Code, out var currency))
            {
                currency.Update(dailyRate.Name, dailyRate.Rate);
                continue;
            }

            _dbContext.Currencies.Add(new Currency(Guid.NewGuid(), dailyRate.Code, dailyRate.Name, dailyRate.Rate));
        }

        var changes = await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Currency rates synchronization completed. Processed: {ProcessedCount}, Changes: {ChangesCount}",
            dailyRatesByCode.Count,
            changes);

        return changes;
    }
}

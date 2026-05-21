using BackgroundWorker.Options;
using BackgroundWorker.Services;
using Microsoft.Extensions.Options;

namespace BackgroundWorker;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly WorkerOptions _options;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory, IOptions<WorkerOptions> options)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromHours(_options.IntervalHours);

        _logger.LogInformation(
            "Currency synchronization worker started. Interval: {Interval}.",
            interval);

        await SynchronizeAsync(stoppingToken);

        using var timer = new PeriodicTimer(interval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await SynchronizeAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Currency synchronization worker is stopping.");
        }
    }

    private async Task SynchronizeAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var synchronizer = scope.ServiceProvider
                .GetRequiredService<CurrencyRatesSynchronizer>();

            var changes = await synchronizer.UpdateRatesAsync(stoppingToken);

            _logger.LogInformation(
                "Synchronization completed at {Time}. Changes: {Changes}.",
                DateTimeOffset.UtcNow,
                changes);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Currency synchronization failed.");
        }
    }
}

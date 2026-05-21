using BackgroundWorker.Models;
using BackgroundWorker.Options;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Xml.Linq;

namespace BackgroundWorker.Services;

public sealed class CurrencyRatesClient
{
    private static readonly CultureInfo CbrCulture = CultureInfo.InvariantCulture;

    private readonly HttpClient _httpClient;
    private readonly ILogger<CurrencyRatesClient> _logger;
    private readonly CurrencyApiOptions _options;

    public CurrencyRatesClient(
        HttpClient httpClient, 
        ILogger<CurrencyRatesClient> logger,
        IOptions<CurrencyApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<CbrCurrencyRate>> GetDailyRatesAsync(CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(_options.DailyRatesPath, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "CBR request failed with status code {StatusCode}",
                response.StatusCode);

            response.EnsureSuccessStatusCode();
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var document = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);

        if (document.Root is null)
        {
            _logger.LogWarning("CBR response XML has no root element");
            return [];
        }

        var rates = document.Root
            .Elements("Valute")
            .Select(ParseCurrencyRate)
            .OfType<CbrCurrencyRate>()
            .ToList();

        rates.Add(new CbrCurrencyRate("RUB", "Russian Ruble", 1m));

        _logger.LogInformation(
            "Fetched {Count} currency rates from CBR",
            rates.Count);

        return rates;
    }

    private CbrCurrencyRate? ParseCurrencyRate(XElement element)
    {
        var code = GetRequiredValue(element, "CharCode");
        var name = GetRequiredValue(element, "Name");
        var nominalText = GetRequiredValue(element, "Nominal")?.Replace(',', '.');
        var valueText = GetRequiredValue(element, "Value")?.Replace(',', '.');

        if (code is null || name is null || nominalText is null || valueText is null)
        {
            _logger.LogWarning("Skipped currency because required XML fields are missing");
            return null;
        }

        if (!decimal.TryParse(nominalText, NumberStyles.Number, CbrCulture, out var nominal) ||
            !decimal.TryParse(valueText, NumberStyles.Number, CbrCulture, out var value))
        {
            _logger.LogWarning(
                "Skipped currency {Code} because rate values are invalid. Nominal: {Nominal}, Value: {Value}",
                code,
                nominalText,
                valueText);

            return null;
        }

        if (nominal <= 0)
        {
            _logger.LogWarning(
                "Skipped currency {Code} because nominal must be greater than zero. Nominal: {Nominal}",
                code,
                nominal);

            return null;
        }

        var rate = Math.Round(value / nominal, 6, MidpointRounding.AwayFromZero);

        return new CbrCurrencyRate(code, name, rate);
    }

    private static string? GetRequiredValue(XElement element, string elementName)
    {
        var value = element.Element(elementName)?.Value?.Trim();

        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
    }
}

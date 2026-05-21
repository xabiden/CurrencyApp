using System.ComponentModel.DataAnnotations;

namespace BackgroundWorker.Options;

public sealed class CurrencyApiOptions
{
    public const string SectionName = "CurrencyApi";

    [Required]
    [Url]
    public string BaseUrl { get; set; } = string.Empty;

    [Range(1, 300)]
    public int TimeoutSeconds { get; set; } = 30;

    [Required]
    public string DailyRatesPath { get; set; } = string.Empty;
}

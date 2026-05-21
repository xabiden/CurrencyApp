using BackgroundWorker.Options;
using BackgroundWorker.Services;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace BackgroundWorker.Extensions
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddCurrencyRatesClient(this IServiceCollection services)
        {
            services.AddSingleton<IConfigureOptions<HttpClientFactoryOptions>, ConfigureCurrencyRatesHttpClient>();

            services.AddHttpClient<CurrencyRatesClient>();

            return services;
        }
    }

    internal sealed class ConfigureCurrencyRatesHttpClient : IConfigureNamedOptions<HttpClientFactoryOptions>
    {
        private readonly CurrencyApiOptions _options;

        public ConfigureCurrencyRatesHttpClient(IOptions<CurrencyApiOptions> options)
        {
            _options = options.Value;
        }

        public void Configure(HttpClientFactoryOptions options) =>
            Configure(nameof(CurrencyRatesClient), options);

        public void Configure(string? name, HttpClientFactoryOptions options)
        {
            if (name != nameof(CurrencyRatesClient)) 
                return;

            options.HttpClientActions.Add(client =>
            {
                client.BaseAddress = new Uri(_options.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
            });
        }
    }
}

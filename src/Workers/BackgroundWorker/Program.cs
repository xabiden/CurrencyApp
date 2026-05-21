using BackgroundWorker;
using BackgroundWorker.Extensions;
using BackgroundWorker.Options;
using BackgroundWorker.Services;
using FinanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddOptions<CurrencyApiOptions>()
    .Bind(builder.Configuration.GetSection(CurrencyApiOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<WorkerOptions>()
    .Bind(builder.Configuration.GetSection(WorkerOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddCurrencyRatesClient();

var financeConnectionString = builder.Configuration.GetConnectionString("FinanceDatabase")
    ?? throw new InvalidOperationException("Connection string 'FinanceDatabase' is not configured.");

builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseNpgsql(financeConnectionString));

builder.Services.AddPostgresHealthCheck(
    builder.Configuration,
    connectionStringName: "FinanceDatabase");

builder.Services.AddScoped<CurrencyRatesSynchronizer>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();

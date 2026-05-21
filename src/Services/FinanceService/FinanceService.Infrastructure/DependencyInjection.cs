using FinanceService.Application.Abstractions.Persistence;
using FinanceService.Infrastructure.Authentication;
using FinanceService.Infrastructure.Persistence;
using FinanceService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Authentication;

namespace FinanceService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration
            .GetConnectionString("FinanceDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'FinanceDatabase' is not configured.");

        services.AddHttpContextAccessor();

        services.AddDbContext<FinanceDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<ICurrencyReadRepository, CurrencyReadRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();

        return services;
    }
}
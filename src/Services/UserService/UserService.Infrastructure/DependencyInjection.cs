using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Authentication;
using UserService.Application.Abstractions.Authentication;
using UserService.Application.Abstractions.Persistence;
using UserService.Infrastructure.Authentication;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Persistence.Repositories;

namespace UserService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var connectionString = configuration
            .GetConnectionString("UserDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'UserDatabase' is not configured."); 

        services.AddHttpContextAccessor();
        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();

        return services;
    }
}

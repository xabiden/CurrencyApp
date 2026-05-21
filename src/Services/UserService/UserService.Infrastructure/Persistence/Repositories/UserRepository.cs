using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstractions.Persistence;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly UserDbContext _dbContext;

    public UserRepository(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _dbContext.Users.Add(user);
        return Task.CompletedTask;
    }

    public async Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Name == name, cancellationToken);
    }
}

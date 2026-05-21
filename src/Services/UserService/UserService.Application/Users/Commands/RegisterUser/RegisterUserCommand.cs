using MediatR;
using Shared.Contracts.Users;
using Shared.Results;
using UserService.Application.Abstractions.Authentication;
using UserService.Application.Abstractions.Persistence;
using UserService.Domain.Entities;

namespace UserService.Application.Users.Commands.RegisterUser;

public sealed record RegisterUserCommand(string Name, string Password) : IRequest<Result<UserDto>>;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserDto>> Handle(
        RegisterUserCommand request, 
        CancellationToken cancellationToken)
    {
        var normalizedName = request.Name.Trim().ToLowerInvariant();

        var existingUser = await _userRepository.GetByNameAsync(normalizedName, cancellationToken);
        if (existingUser is not null)
        {
            return Result<UserDto>.Conflict("Users.AlreadyExists", "User with this name already exists.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = new User(Guid.NewGuid(), normalizedName, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserDto>.Success(new UserDto(user.Id, user.Name));
    }
}

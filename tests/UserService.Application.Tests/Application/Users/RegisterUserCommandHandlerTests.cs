using Shared.Results;
using UserService.Application.Abstractions.Authentication;
using UserService.Application.Abstractions.Persistence;
using UserService.Application.Users.Commands.RegisterUser;
using UserService.Domain.Entities;

namespace UserService.Tests.Application.Users;

[TestFixture]
public sealed class RegisterUserCommandHandlerTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IPasswordHasher> _passwordHasherMock;
    private RegisterUserCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        _handler = new RegisterUserCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object);
    }

    [Test]
    public async Task Handle_NewUser_ReturnsSuccessWithUserDto()
    {
        // Arrange
        var command = new RegisterUserCommand("Denis", "password123");

        _userRepositoryMock
            .Setup(x => x.GetByNameAsync("denis", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _passwordHasherMock
            .Setup(x => x.HashPassword("password123"))
            .Returns("hashed_password");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Name, Is.EqualTo("denis"));
        Assert.That(result.Value.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public async Task Handle_ExistingUser_ReturnsConflictResult()
    {
        // Arrange
        var command = new RegisterUserCommand("Denis", "password123");
        var existingUser = new User(Guid.NewGuid(), "denis", "hashed_password");

        _userRepositoryMock
            .Setup(x => x.GetByNameAsync("denis", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Conflict));
        Assert.That(result.Error.Code, Is.EqualTo("Users.AlreadyExists"));
    }

    [Test]
    public async Task Handle_NewUser_NormalizesNameBeforeSaving()
    {
        // Arrange
        // Имя с пробелами и в верхнем регистре — должно быть нормализовано
        var command = new RegisterUserCommand("  DENIS  ", "password123");

        _userRepositoryMock
            .Setup(x => x.GetByNameAsync("denis", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _passwordHasherMock
            .Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns("hashed_password");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Name, Is.EqualTo("denis"));
    }

    [Test]
    public async Task Handle_NewUser_SavesAndCallsUnitOfWork()
    {
        // Arrange
        var command = new RegisterUserCommand("Denis", "password123");

        _userRepositoryMock
            .Setup(x => x.GetByNameAsync("denis", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _passwordHasherMock
            .Setup(x => x.HashPassword("password123"))
            .Returns("hashed_password");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<User>(u => u.Name == "denis"),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_ExistingUser_NeverCallsAddOrSave()
    {
        // Arrange
        var command = new RegisterUserCommand("Denis", "password123");
        var existingUser = new User(Guid.NewGuid(), "denis", "hashed_password");

        _userRepositoryMock
            .Setup(x => x.GetByNameAsync("denis", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
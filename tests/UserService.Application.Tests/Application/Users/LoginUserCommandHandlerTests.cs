using Shared.Contracts.Users;
using Shared.Results;
using UserService.Application.Abstractions.Authentication;
using UserService.Application.Abstractions.Persistence;
using UserService.Application.Users.Commands.LoginUser;
using UserService.Domain.Entities;

namespace UserService.Tests.Application.Users;

[TestFixture]
public sealed class LoginUserCommandHandlerTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IPasswordHasher> _passwordHasherMock;
    private Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private LoginUserCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();

        _handler = new LoginUserCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenGeneratorMock.Object);
    }

    [Test]
    public async Task Handle_ValidCredentials_ReturnsSuccessWithToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User(userId, "denis", "hashed_password");
        var expectedToken = new AuthTokenResponse("jwt_token", DateTime.UtcNow.AddHours(1));
        var command = new LoginUserCommand("Denis", "password123");

        _userRepositoryMock
            .Setup(x => x.GetByNameAsync("denis", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword("hashed_password", "password123"))
            .Returns(true);

        _jwtTokenGeneratorMock
            .Setup(x => x.GenerateToken(userId, "denis"))
            .Returns(expectedToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Token, Is.EqualTo("jwt_token"));
    }

    [Test]
    public async Task Handle_UserNotFound_ReturnsUnauthorizedResult()
    {
        // Arrange
        var command = new LoginUserCommand("Denis", "password123");

        _userRepositoryMock
            .Setup(x => x.GetByNameAsync("denis", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Unauthorized));
        Assert.That(result.Error.Code, Is.EqualTo("Users.InvalidCredentials"));
    }

    [Test]
    public async Task Handle_WrongPassword_ReturnsUnauthorizedResult()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), "denis", "hashed_password");
        var command = new LoginUserCommand("Denis", "wrong_password");

        _userRepositoryMock
            .Setup(x => x.GetByNameAsync("denis", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword("hashed_password", "wrong_password"))
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Unauthorized));
        Assert.That(result.Error.Code, Is.EqualTo("Users.InvalidCredentials"));
    }

    [Test]
    public async Task Handle_UserNotFound_AndWrongPassword_ReturnSameError()
    {
        // Намеренно одинаковая ошибка — не раскрываем какое поле неверно
        // Arrange
        var userNotFoundCommand = new LoginUserCommand("Unknown", "password123");
        var wrongPasswordCommand = new LoginUserCommand("Denis", "wrong_password");
        var user = new User(Guid.NewGuid(), "denis", "hashed_password");

        _userRepositoryMock
            .Setup(x => x.GetByNameAsync("unknown", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(x => x.GetByNameAsync("denis", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword("hashed_password", "wrong_password"))
            .Returns(false);

        // Act
        var notFoundResult = await _handler.Handle(userNotFoundCommand, CancellationToken.None);
        var wrongPasswordResult = await _handler.Handle(wrongPasswordCommand, CancellationToken.None);

        // Assert
        Assert.That(notFoundResult.Error.Code, Is.EqualTo(wrongPasswordResult.Error.Code));
        Assert.That(notFoundResult.Error.Type, Is.EqualTo(wrongPasswordResult.Error.Type));
    }

    [Test]
    public async Task Handle_ValidCredentials_NormalizesNameBeforeLookup()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), "denis", "hashed_password");
        var command = new LoginUserCommand("  DENIS  ", "password123");

        _userRepositoryMock
            .Setup(x => x.GetByNameAsync("denis", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        _jwtTokenGeneratorMock
            .Setup(x => x.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(new AuthTokenResponse("token", DateTime.UtcNow.AddHours(1)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);

        _userRepositoryMock.Verify(
            x => x.GetByNameAsync("denis", It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
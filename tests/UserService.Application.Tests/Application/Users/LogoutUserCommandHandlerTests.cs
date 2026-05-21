using UserService.Application.Users.Commands.LogoutUser;

namespace UserService.Tests.Application.Users;

[TestFixture]
public sealed class LogoutUserCommandHandlerTests
{
    private LogoutUserCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _handler = new LogoutUserCommandHandler();
    }

    [Test]
    public async Task Handle_Always_ReturnsSuccess()
    {
        // Arrange
        var command = new LogoutUserCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task Handle_Always_IsStateless()
    {
        // JWT logout stateless — два вызова дают одинаковый результат
        // Arrange
        var command = new LogoutUserCommand();

        // Act
        var firstResult = await _handler.Handle(command, CancellationToken.None);
        var secondResult = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(firstResult.IsSuccess, Is.True);
        Assert.That(secondResult.IsSuccess, Is.True);
    }
}
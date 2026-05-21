using Shared.Authentication;
using UserService.Application.Users.Queries.GetCurrentUser;

namespace UserService.Tests.Application.Users;

[TestFixture]
public sealed class GetCurrentUserQueryHandlerTests
{
    private Mock<ICurrentUserContext> _currentUserContextMock;
    private GetCurrentUserQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _currentUserContextMock = new Mock<ICurrentUserContext>();

        _handler = new GetCurrentUserQueryHandler(
            _currentUserContextMock.Object);
    }

    [Test]
    public async Task Handle_AuthenticatedUser_ReturnsSuccessWithUserDto()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _currentUserContextMock.Setup(x => x.UserId).Returns(userId);
        _currentUserContextMock.Setup(x => x.UserName).Returns("denis");

        // Act
        var result = await _handler.Handle(new GetCurrentUserQuery(), CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Id, Is.EqualTo(userId));
        Assert.That(result.Value.Name, Is.EqualTo("denis"));
    }

    [Test]
    public void Handle_UserIdIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        _currentUserContextMock.Setup(x => x.UserId).Returns((Guid?)null);
        _currentUserContextMock.Setup(x => x.UserName).Returns("denis");

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(new GetCurrentUserQuery(), CancellationToken.None));
    }

    [Test]
    public void Handle_UserNameIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        _currentUserContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserContextMock.Setup(x => x.UserName).Returns((string?)null);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(new GetCurrentUserQuery(), CancellationToken.None));
    }
}
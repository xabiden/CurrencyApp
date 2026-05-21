using FinanceService.Application.Abstractions.Persistence;
using FinanceService.Application.Currencies.Queries.GetUserCurrencies;
using Shared.Authentication;
using Shared.Contracts.Finance;

namespace FinanceService.Tests.Application.Currencies;

[TestFixture]
public sealed class GetUserCurrenciesQueryHandlerTests
{
    private Mock<ICurrencyReadRepository> _readRepositoryMock;
    private Mock<ICurrentUserContext> _currentUserContextMock;
    private GetUserCurrenciesQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _readRepositoryMock = new Mock<ICurrencyReadRepository>();
        _currentUserContextMock = new Mock<ICurrentUserContext>();

        _handler = new GetUserCurrenciesQueryHandler(
            _readRepositoryMock.Object,
            _currentUserContextMock.Object);
    }

    [Test]
    public async Task Handle_UserHasFavorites_ReturnsSuccessWithCurrencies()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currencies = new List<UserCurrencyDto>
        {
            new(Guid.NewGuid(), "USD", "US Dollar", 90m, true),
            new(Guid.NewGuid(), "EUR", "Euro", 98m, true)
        };

        _currentUserContextMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _readRepositoryMock
            .Setup(x => x.GetUserCurrenciesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currencies);

        // Act
        var result = await _handler.Handle(new GetUserCurrenciesQuery(), CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Has.Count.EqualTo(2));
        Assert.That(result.Value, Is.EquivalentTo(currencies));
    }

    [Test]
    public async Task Handle_UserHasNoFavorites_ReturnsSuccessWithEmptyCollection()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _currentUserContextMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _readRepositoryMock
            .Setup(x => x.GetUserCurrenciesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserCurrencyDto>());

        // Act
        var result = await _handler.Handle(new GetUserCurrenciesQuery(), CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Empty);
    }

    [Test]
    public void Handle_UserIdIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        _currentUserContextMock
            .Setup(x => x.UserId)
            .Returns((Guid?)null);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(new GetUserCurrenciesQuery(), CancellationToken.None));
    }

    [Test]
    public async Task Handle_ValidUser_CallsRepositoryWithCorrectUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _currentUserContextMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _readRepositoryMock
            .Setup(x => x.GetUserCurrenciesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserCurrencyDto>());

        // Act
        await _handler.Handle(new GetUserCurrenciesQuery(), CancellationToken.None);

        // Assert
        _readRepositoryMock.Verify(
            x => x.GetUserCurrenciesAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
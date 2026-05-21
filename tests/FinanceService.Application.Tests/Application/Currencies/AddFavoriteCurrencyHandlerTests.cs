using FinanceService.Application.Abstractions.Persistence;
using FinanceService.Application.Currencies.Commands.AddFavoriteCurrency;
using FinanceService.Domain.Entities;
using Shared.Authentication;
using Shared.Results;

namespace FinanceService.Tests.Application.Currencies;

[TestFixture]
public sealed class AddFavoriteCurrencyByCodeHandlerTests
{
    private Mock<ICurrencyRepository> _repositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<ICurrentUserContext> _currentUserContextMock;
    private AddFavoriteCurrencyByCodeCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<ICurrencyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserContextMock = new Mock<ICurrentUserContext>();

        _handler = new AddFavoriteCurrencyByCodeCommandHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserContextMock.Object);
    }

    [Test]
    public async Task Handle_ValidCode_ReturnsSuccessWithIsFavoriteTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currency = new Currency(Guid.NewGuid(), "USD", "US Dollar", 90m);
        var command = new AddFavoriteCurrencyByCodeCommand("USD");

        _currentUserContextMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _repositoryMock
            .Setup(x => x.FindByCodeAsync("USD", It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);

        _repositoryMock
            .Setup(x => x.ExistsAsFavoriteAsync(userId, currency.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.IsFavorite, Is.True);
        Assert.That(result.Value.Code, Is.EqualTo("USD"));
    }

    [Test]
    public async Task Handle_CurrencyNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new AddFavoriteCurrencyByCodeCommand("XYZ");

        _currentUserContextMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _repositoryMock
            .Setup(x => x.FindByCodeAsync("XYZ", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Currency?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.NotFound));
        Assert.That(result.Error.Code, Is.EqualTo("Currencies.NotFound"));
    }

    [Test]
    public async Task Handle_AlreadyFavorite_ReturnsConflictResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currency = new Currency(Guid.NewGuid(), "USD", "US Dollar", 90m);
        var command = new AddFavoriteCurrencyByCodeCommand("USD");

        _currentUserContextMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _repositoryMock
            .Setup(x => x.FindByCodeAsync("USD", It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);

        _repositoryMock
            .Setup(x => x.ExistsAsFavoriteAsync(userId, currency.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Conflict));
        Assert.That(result.Error.Code, Is.EqualTo("Currencies.AlreadyFavorite"));
    }

    [Test]
    public void Handle_UserIdIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        _currentUserContextMock
            .Setup(x => x.UserId)
            .Returns((Guid?)null);

        var command = new AddFavoriteCurrencyByCodeCommand("USD");

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_ValidCode_SavesAndCallsUnitOfWork()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currency = new Currency(Guid.NewGuid(), "USD", "US Dollar", 90m);
        var command = new AddFavoriteCurrencyByCodeCommand("USD");

        _currentUserContextMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _repositoryMock
            .Setup(x => x.FindByCodeAsync("USD", It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);

        _repositoryMock
            .Setup(x => x.ExistsAsFavoriteAsync(userId, currency.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            x => x.AddFavoriteAsync(It.Is<UserFavoriteCurrency>(f =>
                f.UserId == userId && f.CurrencyId == currency.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

[TestFixture]
public sealed class AddFavoriteCurrencyByIdHandlerTests
{
    private Mock<ICurrencyRepository> _repositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<ICurrentUserContext> _currentUserContextMock;
    private AddFavoriteCurrencyByIdCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<ICurrencyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserContextMock = new Mock<ICurrentUserContext>();

        _handler = new AddFavoriteCurrencyByIdCommandHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserContextMock.Object);
    }

    [Test]
    public async Task Handle_ValidId_ReturnsSuccessWithIsFavoriteTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currencyId = Guid.NewGuid();
        var currency = new Currency(currencyId, "EUR", "Euro", 98m);
        var command = new AddFavoriteCurrencyByIdCommand(currencyId);

        _currentUserContextMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _repositoryMock
            .Setup(x => x.FindByIdAsync(currencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);

        _repositoryMock
            .Setup(x => x.ExistsAsFavoriteAsync(userId, currencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.IsFavorite, Is.True);
        Assert.That(result.Value.CurrencyId, Is.EqualTo(currencyId));
    }

    [Test]
    public async Task Handle_ValidId_SavesAndCallsUnitOfWork()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currencyId = Guid.NewGuid();
        var currency = new Currency(currencyId, "EUR", "Euro", 98m);
        var command = new AddFavoriteCurrencyByIdCommand(currencyId);

        _currentUserContextMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _repositoryMock
            .Setup(x => x.FindByIdAsync(currencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);

        _repositoryMock
            .Setup(x => x.ExistsAsFavoriteAsync(userId, currencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            x => x.AddFavoriteAsync(
                It.Is<UserFavoriteCurrency>(f =>
                    f.UserId == userId && f.CurrencyId == currencyId),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_AlreadyFavorite_NeverCallsAddOrSave()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currencyId = Guid.NewGuid();
        var currency = new Currency(currencyId, "EUR", "Euro", 98m);
        var command = new AddFavoriteCurrencyByIdCommand(currencyId);

        _currentUserContextMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _repositoryMock
            .Setup(x => x.FindByIdAsync(currencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);

        _repositoryMock
            .Setup(x => x.ExistsAsFavoriteAsync(userId, currencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            x => x.AddFavoriteAsync(
                It.IsAny<UserFavoriteCurrency>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_CurrencyNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currencyId = Guid.NewGuid();
        var command = new AddFavoriteCurrencyByIdCommand(currencyId);

        _currentUserContextMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _repositoryMock
            .Setup(x => x.FindByIdAsync(currencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Currency?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.NotFound));
    }

    [Test]
    public async Task Handle_AlreadyFavorite_ReturnsConflictResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currencyId = Guid.NewGuid();
        var currency = new Currency(currencyId, "EUR", "Euro", 98m);
        var command = new AddFavoriteCurrencyByIdCommand(currencyId);

        _currentUserContextMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _repositoryMock
            .Setup(x => x.FindByIdAsync(currencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);

        _repositoryMock
            .Setup(x => x.ExistsAsFavoriteAsync(userId, currencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Conflict));
    }

    [Test]
    public void Handle_UserIdIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        _currentUserContextMock
            .Setup(x => x.UserId)
            .Returns((Guid?)null);

        var command = new AddFavoriteCurrencyByIdCommand(Guid.NewGuid());

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}
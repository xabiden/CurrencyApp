using FinanceService.Application.Abstractions.Persistence;
using FinanceService.Application.Currencies.Queries.GetAllCurrencies;
using Shared.Contracts.Finance;

namespace FinanceService.Tests.Application.Currencies;

[TestFixture]
public sealed class GetAllCurrenciesQueryHandlerTests
{
    private Mock<ICurrencyReadRepository> _readRepositoryMock;
    private GetAllCurrenciesQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _readRepositoryMock = new Mock<ICurrencyReadRepository>();

        _handler = new GetAllCurrenciesQueryHandler(
            _readRepositoryMock.Object);
    }

    [Test]
    public async Task Handle_Always_ReturnsSuccessWithCurrencies()
    {
        // Arrange
        var currencies = new List<UserCurrencyDto>
        {
            new(Guid.NewGuid(), "EUR", "Euro", 98m, false),
            new(Guid.NewGuid(), "USD", "US Dollar", 90m, false)
        };

        _readRepositoryMock
            .Setup(x => x.GetAllCurrenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(currencies);

        // Act
        var result = await _handler.Handle(new GetAllCurrenciesQuery(), CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Handle_NoCurrencies_ReturnsSuccessWithEmptyCollection()
    {
        // Arrange
        _readRepositoryMock
            .Setup(x => x.GetAllCurrenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserCurrencyDto>());

        // Act
        var result = await _handler.Handle(new GetAllCurrenciesQuery(), CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.Empty);
    }
}
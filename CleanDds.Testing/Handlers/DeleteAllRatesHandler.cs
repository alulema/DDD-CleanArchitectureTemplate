using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanDds.Application.CommandStack.Rates;
using CleanDds.Application.Interfaces;
using CleanDds.Domain.Currencies;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CleanDds.Testing.Handlers;

public class DeleteAllRatesHandlerTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<ILogger<DeleteAllRatesHandler>> _mockLogger;
    private readonly DeleteAllRatesHandler _handler;

    public DeleteAllRatesHandlerTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockLogger = new Mock<ILogger<DeleteAllRatesHandler>>();
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(sp => sp.GetService(typeof(ILogger<DeleteAllRatesHandler>)))
            .Returns(_mockLogger.Object);

        _handler = new DeleteAllRatesHandler(serviceProvider.Object, _mockDatabaseService.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteAllRates()
    {
        // Arrange
        SetupMockRates();

        // Act
        await _handler.Handle(new DeleteAllRates(), CancellationToken.None);

        // Assert
        _mockDatabaseService.Verify(db => db.Rates.RemoveRange(It.IsAny<IEnumerable<Rate>>()), Times.Once);
        _mockDatabaseService.Verify(db => db.Save(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogInformation()
    {
        // Arrange
        SetupMockRates();

        // Act
        await _handler.Handle(new DeleteAllRates(), CancellationToken.None);

        // Assert
        _mockLogger.Verify(log => log.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Executing 'Delete All Rates' Command")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenExceptionThrown_ShouldLogErrorAndRethrow()
    {
        // Arrange
        _mockDatabaseService.Setup(db => db.Rates).Throws(new Exception("Database Error"));

        // Act
        Func<Task> act = async () => await _handler.Handle(new DeleteAllRates(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database Error");
        _mockLogger.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error executing 'Delete All Rates' command")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }
    
    private void SetupMockRates()
    {
        var rates = new List<Rate> { new(), new() }.AsQueryable();
        var mockSet = new Mock<DbSet<Rate>>();
        mockSet.As<IQueryable<Rate>>().Setup(m => m.Provider).Returns(rates.Provider);
        mockSet.As<IQueryable<Rate>>().Setup(m => m.Expression).Returns(rates.Expression);
        mockSet.As<IQueryable<Rate>>().Setup(m => m.ElementType).Returns(rates.ElementType);
        mockSet.As<IQueryable<Rate>>().Setup(m => m.GetEnumerator()).Returns(() => rates.GetEnumerator());

        _mockDatabaseService.Setup(db => db.Rates).Returns(mockSet.Object);
    }
}
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

public class SaveRatesHandlerTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<ILogger<SaveRatesHandler>> _mockLogger;
    private readonly SaveRatesHandler _handler;

    public SaveRatesHandlerTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockLogger = new Mock<ILogger<SaveRatesHandler>>();
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(sp => sp.GetService(typeof(ILogger<SaveRatesHandler>)))
            .Returns(_mockLogger.Object);

        _handler = new SaveRatesHandler(serviceProvider.Object, _mockDatabaseService.Object);
    }

    [Fact]
    public async Task Handle_ShouldSaveRates()
    {
        // Arrange
        SetupMockRates();
        var rates = new List<Rate>{ new(), new() };
        var request = new SaveRates { Rates = rates.ToArray() };

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mockDatabaseService.Verify(db => db.Rates.Add(It.IsAny<Rate>()), Times.Exactly(rates.Count));
        _mockDatabaseService.Verify(db => db.Save(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogInformation()
    {
        // Arrange
        SetupMockRates();
        var rates = new List<Rate> { new(), new() };
        var request = new SaveRates { Rates = rates.ToArray() };

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mockLogger.Verify(log => log.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Executing 'Save Rates' Command")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenExceptionThrown_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var rates = new List<Rate> { new(), new() };
        var request = new SaveRates { Rates = rates.ToArray() };
        _mockDatabaseService.Setup(db => db.Rates.Add(It.IsAny<Rate>())).Throws(new Exception("Database Error"));

        // Act
        Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database Error");
        _mockLogger.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error executing 'Save Rates' command")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }
    
    private void SetupMockRates()
    {
        var rates = new List<Rate>().AsQueryable();
        var mockSet = new Mock<DbSet<Rate>>();
        mockSet.As<IQueryable<Rate>>().Setup(m => m.Provider).Returns(rates.Provider);
        mockSet.As<IQueryable<Rate>>().Setup(m => m.Expression).Returns(rates.Expression);
        mockSet.As<IQueryable<Rate>>().Setup(m => m.ElementType).Returns(rates.ElementType);
        mockSet.As<IQueryable<Rate>>().Setup(m => m.GetEnumerator()).Returns(() => rates.GetEnumerator());

        _mockDatabaseService.Setup(db => db.Rates).Returns(mockSet.Object);
    }
}

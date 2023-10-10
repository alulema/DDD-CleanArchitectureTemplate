using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanDds.Application.CommandStack.Transactions;
using CleanDds.Application.Interfaces;
using CleanDds.Domain.Currencies;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CleanDds.Testing.Handlers;

public class SaveTransactionsHandlerTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<ILogger<SaveTransactionsHandler>> _mockLogger;
    private readonly SaveTransactionsHandler _handler;

    public SaveTransactionsHandlerTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockLogger = new Mock<ILogger<SaveTransactionsHandler>>();
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(sp => sp.GetService(typeof(ILogger<SaveTransactionsHandler>)))
            .Returns(_mockLogger.Object);

        _handler = new SaveTransactionsHandler(serviceProvider.Object, _mockDatabaseService.Object);
    }

    [Fact]
    public async Task Handle_ShouldSaveTransactions()
    {
        // Arrange
        var transactions = new List<Transaction> { new(), new() }.ToArray();
        SetupMockTransactions();

        // Act
        await _handler.Handle(new SaveTransactions { Transactions = transactions }, CancellationToken.None);

        // Assert
        _mockDatabaseService.Verify(db => db.Transactions.Add(It.IsAny<Transaction>()), Times.Exactly(transactions.Length));
        _mockDatabaseService.Verify(db => db.Save(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogInformation()
    {
        // Act
        await _handler.Handle(new SaveTransactions { Transactions = new List<Transaction>().ToArray() }, CancellationToken.None);

        // Assert
        _mockLogger.Verify(log => log.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Executing 'Save Transactions' Command")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenExceptionThrown_ShouldLogErrorAndRethrow()
    {
        // Arrange
        _mockDatabaseService.Setup(db => db.Transactions.Add(It.IsAny<Transaction>())).Throws(new Exception("Database Error"));

        // Act
        Func<Task> act = async () => await _handler.Handle(new SaveTransactions { Transactions = new List<Transaction> { new() }.ToArray() }, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database Error");
        _mockLogger.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error executing 'Save Transactions' command")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }
    
    private void SetupMockTransactions()
    {
        var queryableRates = new List<Transaction>().AsQueryable();
        var mockSet = new Mock<DbSet<Transaction>>();
        mockSet.As<IQueryable<Transaction>>().Setup(m => m.Provider).Returns(queryableRates.Provider);
        mockSet.As<IQueryable<Transaction>>().Setup(m => m.Expression).Returns(queryableRates.Expression);
        mockSet.As<IQueryable<Transaction>>().Setup(m => m.ElementType).Returns(queryableRates.ElementType);
        mockSet.As<IQueryable<Transaction>>().Setup(m => m.GetEnumerator()).Returns(() => queryableRates.GetEnumerator());

        _mockDatabaseService.Setup(db => db.Transactions).Returns(mockSet.Object);
    }
}

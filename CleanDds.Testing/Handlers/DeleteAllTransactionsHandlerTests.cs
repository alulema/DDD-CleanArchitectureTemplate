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

public class DeleteAllTransactionsHandlerTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<ILogger<DeleteAllTransactionsHandler>> _mockLogger;
    private readonly DeleteAllTransactionsHandler _handler;

    public DeleteAllTransactionsHandlerTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockLogger = new Mock<ILogger<DeleteAllTransactionsHandler>>();
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(sp => sp.GetService(typeof(ILogger<DeleteAllTransactionsHandler>)))
            .Returns(_mockLogger.Object);

        _handler = new DeleteAllTransactionsHandler(serviceProvider.Object, _mockDatabaseService.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteAllTransactions()
    {
        // Arrange
        SetupMockTransactions(new List<Transaction> { new(), new() });

        // Act
        await _handler.Handle(new DeleteAllTransactions(), CancellationToken.None);

        // Assert
        _mockDatabaseService.Verify(db => db.Transactions.RemoveRange(It.IsAny<IEnumerable<Transaction>>()), Times.Once);
        _mockDatabaseService.Verify(db => db.Save(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogInformation()
    {
        SetupMockTransactions();

        // Act
        await _handler.Handle(new DeleteAllTransactions(), CancellationToken.None);

        // Assert
        _mockLogger.Verify(log => log.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Executing 'Delete All Transactions' Command")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenExceptionThrown_ShouldLogErrorAndRethrow()
    {
        // Arrange
        _mockDatabaseService.Setup(db => db.Transactions).Throws(new Exception("Database Error"));

        // Act
        Func<Task> act = async () => await _handler.Handle(new DeleteAllTransactions(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database Error");
        _mockLogger.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error executing 'Delete All Transactions' command")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }
    
    private void SetupMockTransactions(IEnumerable<Transaction> rates = null)
    {
        rates ??= new List<Transaction>();
        var queryableRates = rates.AsQueryable();
        var mockSet = new Mock<DbSet<Transaction>>();
        mockSet.As<IQueryable<Transaction>>().Setup(m => m.Provider).Returns(queryableRates.Provider);
        mockSet.As<IQueryable<Transaction>>().Setup(m => m.Expression).Returns(queryableRates.Expression);
        mockSet.As<IQueryable<Transaction>>().Setup(m => m.ElementType).Returns(queryableRates.ElementType);
        mockSet.As<IQueryable<Transaction>>().Setup(m => m.GetEnumerator()).Returns(() => queryableRates.GetEnumerator());

        _mockDatabaseService.Setup(db => db.Transactions).Returns(mockSet.Object);
    }
}

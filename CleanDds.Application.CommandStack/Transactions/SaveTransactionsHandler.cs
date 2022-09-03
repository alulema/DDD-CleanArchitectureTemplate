using System;
using System.Threading;
using System.Threading.Tasks;
using CleanDds.Application.Interfaces;
using CleanDds.Persistance;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanDds.Application.CommandStack.Transactions;

public class SaveTransactionsHandler : IRequestHandler<SaveTransactions>
{
    private readonly IDatabaseService _database;
    private readonly ILogger _logger;

    public SaveTransactionsHandler(IServiceProvider serviceProvider, InMemDatabaseService database)
    {
        _database = database;
        _logger = serviceProvider.GetService<ILogger<SaveTransactionsHandler>>();
    }

    public Task<Unit> Handle(SaveTransactions request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing 'Save Transactions' Command");

        try
        {
            foreach (var transaction in request.Transactions)
                _database.Transactions.Add(transaction);

            _database.Save();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing 'Save Transactions' command");
            throw;
        }

        return Unit.Task;
    }
}
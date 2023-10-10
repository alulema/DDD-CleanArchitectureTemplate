using System;
using System.Threading;
using System.Threading.Tasks;
using CleanDds.Application.Interfaces;
using CleanDds.Persistance;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanDds.Application.CommandStack.Transactions;

public class DeleteAllTransactionsHandler : IRequestHandler<DeleteAllTransactions>
{
    private readonly IDatabaseService _database;
    private readonly ILogger _logger;

    public DeleteAllTransactionsHandler(IServiceProvider serviceProvider, IDatabaseService database)
    {
        _database = database;
        _logger = serviceProvider.GetService<ILogger<DeleteAllTransactionsHandler>>();
    }

    public Task<Unit> Handle(DeleteAllTransactions request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing 'Delete All Transactions' Command");

        try
        {
            _database.Transactions.RemoveRange(_database.Transactions);
            _database.Save();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error executing 'Delete All Transactions' command");
            throw;
        }

        return Unit.Task;
    }
}

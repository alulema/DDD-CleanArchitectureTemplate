using System;
using System.Threading;
using System.Threading.Tasks;
using CleanDds.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanDds.Application.CommandStack.Rates;

public class DeleteAllRatesHandler : IRequestHandler<DeleteAllRates>
{
    private readonly IDatabaseService _database;
    private readonly ILogger _logger;

    public DeleteAllRatesHandler(IServiceProvider serviceProvider, IDatabaseService database)
    {
        _database = database;
        _logger = serviceProvider.GetService<ILogger<DeleteAllRatesHandler>>();
    }

    public Task Handle(DeleteAllRates request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing 'Delete All Rates' Command");

        try
        {
            _database.Rates.RemoveRange(_database.Rates);
            _database.Save();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing 'Delete All Rates' command");
            throw;
        }

        return Unit.Task;
    }
}

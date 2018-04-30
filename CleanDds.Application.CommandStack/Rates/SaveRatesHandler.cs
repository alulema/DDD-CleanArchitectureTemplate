using System;
using System.Threading;
using System.Threading.Tasks;
using CleanDds.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanDds.Application.CommandStack.Rates
{
    public class SaveRatesHandler : IRequestHandler<SaveRates>
    {
        private readonly IDatabaseService _database;
        private readonly ILogger _logger;

        public SaveRatesHandler(IServiceProvider serviceProvider)
        {
            _database = serviceProvider.GetService<IDatabaseService>();
            _logger = serviceProvider.GetService<ILogger<SaveRatesHandler>>();
        }

        public Task Handle(SaveRates request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executing 'Save Rates' Command");

            try
            {
                foreach (var rate in request.Rates)
                    _database.Rates.Add(rate);

                _database.Save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing 'Save Rates' command");
                throw;
            }

            return Task.CompletedTask;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanDds.Application.Interfaces;
using CleanDds.Domain.Currencies;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanDds.Application.QueryStack.Rates
{
    public class GetRatesListHandler : IRequestHandler<GetRatesList, List<Rate>>
    {
        private readonly IDatabaseService _database;
        private readonly ILogger _logger;

        public GetRatesListHandler(IServiceProvider serviceProvider)
        {
            _database = serviceProvider.GetService<IDatabaseService>();
            _logger = serviceProvider.GetService<ILogger<GetRatesListHandler>>();
        }

        public Task<List<Rate>> Handle(GetRatesList request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting Rates List");

            try
            {
                return _database.Rates.ToListAsync(cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Getting Rates List");
                throw;
            }
        }
    }
}
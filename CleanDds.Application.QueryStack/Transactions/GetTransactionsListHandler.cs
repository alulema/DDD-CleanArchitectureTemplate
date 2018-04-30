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

namespace CleanDds.Application.QueryStack.Transactions
{
    public class GetTransactionsListHandler : IRequestHandler<GetTransactionsList, List<Transaction>>
    {
        private readonly IDatabaseService _database;
        private readonly ILogger _logger;

        public GetTransactionsListHandler(IServiceProvider serviceProvider)
        {
            _database = serviceProvider.GetService<IDatabaseService>();
            _logger = serviceProvider.GetService<ILogger<GetTransactionsListHandler>>();            
        }
        
        public Task<List<Transaction>> Handle(GetTransactionsList request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting Transactions List");

            try
            {
                return _database.Transactions.ToListAsync(cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Getting Transactions List");
                throw;
            }
        }
    }
}
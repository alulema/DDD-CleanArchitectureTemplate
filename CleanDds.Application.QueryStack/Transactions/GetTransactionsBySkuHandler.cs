using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanDds.Application.Interfaces;
using CleanDds.Application.ViewModels.Transactions;
using CleanDds.Common.Numbers;
using CleanDds.Domain.Common;
using CleanDds.Domain.Currencies;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanDds.Application.QueryStack.Transactions;

public class GetTransactionsBySkuHandler : IRequestHandler<GetTransactionsBySku, TransactionsBySkuModel>
{
    private readonly IDatabaseService _database;
    private readonly ILogger _logger;

    public GetTransactionsBySkuHandler(IServiceProvider serviceProvider)
    {
        _database = serviceProvider.GetService<IDatabaseService>();
        _logger = serviceProvider.GetService<ILogger<GetTransactionsBySkuHandler>>();            
    }
    
    public Task<TransactionsBySkuModel> Handle(GetTransactionsBySku request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Getting Transactions for SKU {request.Sku}");
        
        try
        {
            var finalTransactions = new List<Transaction>();
            var transactions = _database.Transactions.Where(x => x.Sku == request.Sku).ToList();
            var rates = _database.Rates.ToList();

            foreach (var tx in transactions)
            {
                if (tx.Currency == CurrencyType.EUR)
                {
                    finalTransactions.Add(tx);
                    continue;
                }

                var rate = rates.First(x => x.CurrencyFrom == tx.Currency && x.CurrencyTo == CurrencyType.EUR);
                finalTransactions.Add(new Transaction
                {
                    Currency = CurrencyType.EUR,
                    Sku = request.Sku,
                    Id = Guid.NewGuid(),
                    Amount = Math.Round(Util.RoundHalfToEven(tx.Amount * rate.CurrencyRate), 2)
                });
            }
            
            var response = new TransactionsBySkuModel
            {
                Total = Math.Round(finalTransactions.Sum(x => x.Amount), 2),
                Transactions = finalTransactions.ToArray()
            };

            return Task.FromResult(response);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error Getting Transactions for SKU {request.Sku}");
            throw;
        }
    }
}
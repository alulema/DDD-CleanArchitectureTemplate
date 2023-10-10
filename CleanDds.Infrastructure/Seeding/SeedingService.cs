using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CleanDds.Application.CommandStack.Rates;
using CleanDds.Application.CommandStack.Transactions;
using CleanDds.Application.Interfaces;
using CleanDds.Domain.Common;
using CleanDds.Domain.Currencies;

namespace CleanDds.Infrastructure.Seeding;

public class SeedingService : ISeedingService
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;

    public SeedingService(ILogger<SeedingService> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task SeedRates(string ratesUrl)
    {
        _logger.LogInformation("Starting seeding rates from web service");

        try
        {
            List<Rate> additionalRates = new List<Rate>();
            // HttpClient client = new HttpClient();
            // var json = await client.GetStringAsync(ratesUrl);
            // var rates = JsonConvert.DeserializeObject<Rate[]>(json);
            var rates = new[]
            {
                new Rate {CurrencyFrom = CurrencyType.AUD, CurrencyTo = CurrencyType.CAD, CurrencyRate = 1.0079m},
                new Rate {CurrencyFrom = CurrencyType.CAD, CurrencyTo = CurrencyType.USD, CurrencyRate = 1.0090m}
            };

            foreach (var rate in rates)
                rate.Id = Guid.NewGuid();

            var currencies = new[] { CurrencyType.AUD, CurrencyType.CAD, CurrencyType.EUR, CurrencyType.USD };

            for (int i = 0; i < currencies.Length; i++)
            {
                var currencyFrom = currencies[i];
                var isInRates = (from x in rates
                    where x.CurrencyFrom == currencyFrom
                    select x).ToArray();

                for (int j = 0; j < currencies.Length; j++)
                {
                    var currencyTo = currencies[j];
                    if (j == i) continue;

                    if (isInRates.Any(x => x.CurrencyTo == currencyTo))
                        continue;

                    ExploreNode(1, rates, isInRates, additionalRates, currencyFrom, currencyTo, currencyFrom);
                }
            }

            additionalRates.AddRange(rates);
            await _mediator.Send(new SaveRates
            {
                Rates = additionalRates.ToArray()
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error seeding rates from web service");
            throw;
        }
    }

    private void ExploreNode(decimal incrementalRatio, Rate[] allRates, Rate[] isInRates, List<Rate> additionalRates, CurrencyType currencyFrom, CurrencyType currencyTo, CurrencyType originalFrom)
    {
        foreach (var rate in isInRates)
        {
            var currentFrom = rate.CurrencyTo;
            var currentInRates = (from x in allRates
                                  where x.CurrencyFrom == currentFrom
                                  select x).ToArray();

            if (currentInRates.Any(x => x.CurrencyTo == currencyTo))
            {
                var w = currentInRates.First(x => x.CurrencyTo == currencyTo);
                additionalRates.Add(new Rate
                {
                    CurrencyFrom = originalFrom,
                    CurrencyTo = w.CurrencyTo,
                    CurrencyRate = incrementalRatio * rate.CurrencyRate * w.CurrencyRate,
                    Id = Guid.NewGuid()
                });

                return;
            }

            ExploreNode(incrementalRatio * rate.CurrencyRate, allRates, currentInRates, additionalRates, currentFrom, currencyTo, originalFrom);
        }
    }

    public async Task SeedTransactions(string transactionsUrl)
    {
        _logger.LogInformation("Starting seeding transactions from web service");

        try
        {
            // HttpClient client = new HttpClient();
            // var json = await client.GetStringAsync(transactionsUrl);
            // var transactions = JsonConvert.DeserializeObject<Transaction[]>(json);
            var transactions = new[]
            {
                new Transaction {Sku = "A", Amount = 10, Currency = CurrencyType.USD},
                new Transaction {Sku = "B", Amount = 15, Currency = CurrencyType.CAD}
            };

            foreach (var transaction in transactions)
                transaction.Id = Guid.NewGuid();

            await _mediator.Send(new SaveTransactions
            {
                Transactions = transactions
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error seeding transactions from web service");
            throw;
        }
    }
}

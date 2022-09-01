using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanDds.Application.QueryStack.Transactions;
using CleanDds.Application.ViewModels.Transactions;
using CleanDds.Domain.Currencies;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CleanDds.Service.Controllers;

[Route("api/[controller]")]
public class TransactionsController : Controller
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;

    public TransactionsController(ILogger<TransactionsController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IEnumerable<Transaction>> Get()
    {
        _logger.LogInformation("Getting all transactions");

        try
        {
            var transactions = await _mediator.Send(new GetTransactionsList());
            return transactions;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting transactions from In_Mem database");
            throw;
        }
    }

    [HttpGet("{sku}", Name = "Get")]
    public async Task<TransactionsBySkuModel> Get(string sku)
    {
        _logger.LogInformation($"Getting transactions for SKU {sku}");

        try
        {
            var response = await _mediator.Send(new GetTransactionsBySku
            {
                Sku = sku
            });
            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error getting transactions for SKU {sku}");
            throw;
        }
    }
}
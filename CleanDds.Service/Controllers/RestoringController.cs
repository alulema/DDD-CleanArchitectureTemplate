using System;
using CleanDds.Application.CommandStack.Rates;
using CleanDds.Application.CommandStack.Transactions;
using CleanDds.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CleanDds.Service.Controllers;

[Route("api/[controller]")]
public class RestoringController : Controller
{
    private readonly ISeedingService _seedingService;
    private readonly ILogger _logger;
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public RestoringController(ISeedingService seedingService, ILogger<RestoringController> logger, IMediator mediator, IConfiguration configuration)
    {
        _seedingService = seedingService;
        _logger = logger;
        _mediator = mediator;
        _configuration = configuration;
    }

    [HttpGet]
    public bool Get()
    {
        _logger.LogInformation("Deleting all data and restoring again from Web Services");

        try
        {
            _mediator.Send(new DeleteAllTransactions());
            _mediator.Send(new DeleteAllRates());
            _seedingService.SeedRates(_configuration["RatesUrl"]);
            _seedingService.SeedTransactions(_configuration["TransactionsUrl"]);

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error restoring data in In-Mem database");
            throw;
        }
    }
}
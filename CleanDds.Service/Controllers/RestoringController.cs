using System;
using CleanDds.Application.CommandStack.Rates;
using CleanDds.Application.CommandStack.Transactions;
using CleanDds.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CleanDds.Service.Controllers
{
    [Route("api/[controller]")]
    public class RestoringController : Controller
    {
        private readonly ISeedingService _seedingService;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public RestoringController(ISeedingService seedingService, ILogger<RestoringController> logger, IMediator mediator)
        {
            _seedingService = seedingService;
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public bool Get()
        {
            _logger.LogInformation("Deleting all data and restoring again from Web Services");

            try
            {
                _mediator.Send(new DeleteAllTransactions());
                _mediator.Send(new DeleteAllRates());
                _seedingService.SeedRates(Program.Configuration["RatesUrl"]);
                _seedingService.SeedTransactions(Program.Configuration["TransactionsUrl"]);

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error restoring data in In-Mem database");
                throw;
            }
        }
    }
}
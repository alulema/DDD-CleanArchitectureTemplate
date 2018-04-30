using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanDds.Application.QueryStack.Rates;
using CleanDds.Domain.Currencies;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CleanDds.Service.Controllers
{
    [Route("api/[controller]")]
    public class RatesController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public RatesController(ILogger<RatesController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        
        [HttpGet]
        public async Task<IEnumerable<Rate>> Index()
        {
            _logger.LogInformation("Getting all rates");

            try
            {
                var rates = await _mediator.Send(new GetRatesList());
                return rates;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting rates from In_Mem database");
                throw;
            }
        }
    }
}
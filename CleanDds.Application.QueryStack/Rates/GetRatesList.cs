using System.Collections.Generic;
using CleanDds.Domain.Currencies;
using MediatR;

namespace CleanDds.Application.QueryStack.Rates;

public class GetRatesList : IRequest<List<Rate>>
{
}
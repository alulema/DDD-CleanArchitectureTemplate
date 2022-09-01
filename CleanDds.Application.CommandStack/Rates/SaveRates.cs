using CleanDds.Domain.Currencies;
using MediatR;

namespace CleanDds.Application.CommandStack.Rates;

public class SaveRates : IRequest
{
    public Rate[] Rates { get; set; }
}

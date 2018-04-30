using System.Collections.Generic;
using CleanDds.Domain.Currencies;
using MediatR;

namespace CleanDds.Application.QueryStack.Transactions
{
    public class GetTransactionsList : IRequest<List<Transaction>>
    {        
    }
}
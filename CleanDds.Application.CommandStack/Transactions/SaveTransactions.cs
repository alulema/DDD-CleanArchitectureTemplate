using CleanDds.Domain.Currencies;
using MediatR;

namespace CleanDds.Application.CommandStack.Transactions
{
    public class SaveTransactions : IRequest
    {
        public Transaction[] Transactions { get; set; }
    }
}
using CleanDds.Application.ViewModels.Transactions;
using MediatR;

namespace CleanDds.Application.QueryStack.Transactions;

public class GetTransactionsBySku : IRequest<TransactionsBySkuModel>
{
    public string Sku { get; set; }
}
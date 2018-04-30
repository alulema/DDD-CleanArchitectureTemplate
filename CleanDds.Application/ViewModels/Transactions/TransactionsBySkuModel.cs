using System.Runtime.Serialization;
using CleanDds.Domain.Currencies;

namespace CleanDds.Application.ViewModels.Transactions
{
    [DataContract]
    public class TransactionsBySkuModel
    {
        [DataMember(Name = "total")]
        public decimal Total { get; set; }

        [DataMember(Name = "transactions")]
        public Transaction[] Transactions { get; set; }
    }
}

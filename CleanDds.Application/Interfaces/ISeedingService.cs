using System.Threading.Tasks;

namespace CleanDds.Application.Interfaces
{
    public interface ISeedingService
    {
        Task SeedRates(string ratesUrl);
        Task SeedTransactions(string transactionsUrl);
    }
}

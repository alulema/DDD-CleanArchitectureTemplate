using System.Threading.Tasks;
using CleanDds.Domain.Currencies;
using Microsoft.EntityFrameworkCore;

namespace CleanDds.Application.Interfaces;

public interface IDatabaseService
{
    DbSet<Rate> Rates { get; set; }
    DbSet<Transaction> Transactions { get; set; }
    Task SaveAsync();
    void Save();
}

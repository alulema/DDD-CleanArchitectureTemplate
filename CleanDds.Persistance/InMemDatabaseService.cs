using System;
using System.Threading.Tasks;
using CleanDds.Application.Interfaces;
using CleanDds.Domain.Currencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanDds.Persistance;

public class InMemDatabaseService : DbContext, IDatabaseService
{
    private readonly ILogger _logger;
    public DbSet<Rate> Rates { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public InMemDatabaseService(DbContextOptions<InMemDatabaseService> options, ILogger<InMemDatabaseService> logger) : base(options)
    {
        _logger = logger;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("in-mem");
    }

    public async Task SaveAsync()
    {
        _logger.LogInformation("Asynchronously saving data to in-mem database");

        try
        {
            await SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error saving data");
            throw;
        }
    }

    public void Save()
    {
        _logger.LogInformation("Saving data to in-mem database");

        try
        {
            SaveChanges();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error seesavingding data");
            throw;
        }
    }
}
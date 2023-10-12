using System.Reflection;
using CleanDds.Application.Interfaces;
using CleanDds.Infrastructure.Seeding;
using CleanDds.Persistance;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<InMemDatabaseService>(x => x.UseInMemoryDatabase("in-mem"));
builder.Services.AddTransient<ISeedingService, SeedingService>();
builder.Services.AddScoped<IDatabaseService, InMemDatabaseService>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    Assembly.Load("CleanDds.Application.CommandStack"),
    Assembly.Load("CleanDds.Application.QueryStack")
));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    var seeding = scope.ServiceProvider.GetService<ISeedingService>();

    if (seeding is not null)
    {
        await seeding.SeedRates(app.Configuration["RatesUrl"]);
        await seeding.SeedTransactions(app.Configuration["TransactionsUrl"]);
    }
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

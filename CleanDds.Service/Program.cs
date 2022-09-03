using System.Reflection;
using CleanDds.Application.Interfaces;
using CleanDds.Infrastructure.Seeding;
using CleanDds.Persistance;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<InMemDatabaseService>(x => x.UseInMemoryDatabase("in-mem"));
builder.Services.AddTransient<ISeedingService, SeedingService>();
builder.Services.AddScoped<IDatabaseService, InMemDatabaseService>();

builder.Services.AddMediatR(new[]
{
    Assembly.Load("CleanDds.Application.CommandStack"),
    Assembly.Load("CleanDds.Application.QueryStack")
});

var app = builder.Build();

// Configure the HTTP request pipeline.

var seeding = app.Services.GetService<ISeedingService>();

if (seeding is not null)
{
    seeding.SeedRates(app.Configuration["RatesUrl"]);
    seeding.SeedTransactions(app.Configuration["TransactionsUrl"]);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

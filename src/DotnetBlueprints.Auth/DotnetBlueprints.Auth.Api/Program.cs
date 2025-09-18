using DotnetBlueprints.Auth.Infrastructure;
using DotnetBlueprints.Auth.Api;
using DotnetBlueprints.Auth.Application;
using Serilog;
using DotnetBlueprints.Sales.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using DotnetBlueprints.Auth.Infrastructure.Persistence;
using DotnetBlueprints.SharedKernel.Infrastructure;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

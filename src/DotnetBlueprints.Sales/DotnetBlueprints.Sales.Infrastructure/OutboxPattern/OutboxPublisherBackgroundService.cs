using DotnetBlueprints.Sales.Application.Common.Interfaces;
using DotnetBlueprints.Sales.Domain;
using DotnetBlueprints.Sales.Domain.Entities;
using DotnetBlueprints.SharedKernel.Exceptions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DotnetBlueprints.Sales.Infrastructure.OutboxPattern;

public class OutboxPublisherBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<OutboxPublisherBackgroundService> _logger;

    public OutboxPublisherBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        IPublishEndpoint publishEndpoint,
        ILogger<OutboxPublisherBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISalesDbContext>();

            var events = context.OutboxMessages
                .Where(x => !x.Processed)
                .OrderBy(x => x.OccurredOn)
                .Take(20)
                .ToList();

            foreach (var outbox in events)
            {
                try
                {
                    if (outbox.Type == nameof(Offer))
                    {
                        var offer = JsonSerializer.Deserialize<Offer>(outbox.Content);
                        if (offer != null)
                            await _publishEndpoint.Publish(offer, stoppingToken);
                    }
                    else if (outbox.Type == nameof(OfferItem))
                    {
                        var offerItem = JsonSerializer.Deserialize<OfferItem>(outbox.Content);
                        if (offerItem != null)
                            await _publishEndpoint.Publish(offerItem, stoppingToken);
                    }

                    outbox.Processed = true;
                    outbox.ProcessedOn = DateTime.UtcNow;
                    context.OutboxMessages.Update(outbox);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Outbox exception");
                }
            }

            await context.SaveChangesAsync(stoppingToken);
            await Task.Delay(5000, stoppingToken);
        }
    }
}

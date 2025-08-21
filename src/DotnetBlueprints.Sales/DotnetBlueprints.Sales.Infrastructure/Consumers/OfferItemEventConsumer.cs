using DotnetBlueprints.Elastic.Services;
using DotnetBlueprints.Sales.Domain.Entities;
using MassTransit;

namespace DotnetBlueprints.Sales.Infrastructure.Consumers;

public class OfferItemEventConsumer : IConsumer<OfferItem>
{
    private readonly IElasticService _elasticService;

    public OfferItemEventConsumer(IElasticService elasticService)
    {
        _elasticService = elasticService;
    }

    public async Task Consume(ConsumeContext<OfferItem> context)
    {
        await _elasticService.IndexAsync(context.Message);
    }
}

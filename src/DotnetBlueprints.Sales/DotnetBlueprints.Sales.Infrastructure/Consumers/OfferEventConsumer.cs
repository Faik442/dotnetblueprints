using DotnetBlueprints.Elastic.Services;
using DotnetBlueprints.Sales.Domain.Entities;
using MassTransit;

namespace DotnetBlueprints.Sales.Infrastructure.Consumers;

public class OfferEventConsumer : IConsumer<Offer>
{
    private readonly IElasticService _elasticService;

    public OfferEventConsumer(IElasticService elasticService)
    {
        _elasticService = elasticService;
    }

    public async Task Consume(ConsumeContext<Offer> context)
    {
        await _elasticService.IndexAsync(context.Message);
    }
}
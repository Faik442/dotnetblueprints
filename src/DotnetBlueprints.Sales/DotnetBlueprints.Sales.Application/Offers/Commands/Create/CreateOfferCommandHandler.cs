using DotnetBlueprints.Sales.Application.Common.Interfaces;
using DotnetBlueprints.Sales.Domain;
using DotnetBlueprints.Sales.Domain.Entities;
using MassTransit;
using MediatR;
using System.Text.Json;

namespace DotnetBlueprints.Sales.Application.Offers.Commands.Create;

public class CreateOfferCommandHandler : IRequestHandler<CreateOfferCommand, Guid>
{
    private readonly ISalesDbContext _context;
    public CreateOfferCommandHandler(ISalesDbContext context)
    {
        _context = context;
    }
    public async Task<Guid> Handle(CreateOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = new Offer(request.Title, request.ValidUntil, request.CreatedBy);

        using var tx = await _context.BeginTransactionAsync(cancellationToken);

        _context.Offers.Add(offer);

        var outbox = new OutboxMessage(nameof(Offer), JsonSerializer.Serialize(offer));

        _context.OutboxMessages.Add(outbox);
        await _context.SaveChangesAsync(cancellationToken);

        await tx.CommitAsync(cancellationToken);

        return offer.Id;
    }
}

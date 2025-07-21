using DotnetBlueprints.Sales.Application.Common.Interfaces;
using DotnetBlueprints.Sales.Domain.Entities;
using MediatR;

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
        var offer = new Offer(request.Title, request.ValidUntil);

        foreach (var item in request.Items)
        {
            offer.AddItem(item.Name, item.Quantity, item.UnitPrice);
        }

        _context.Offers.Add(offer);
        await _context.SaveChangesAsync(cancellationToken);

        return offer.Id;
    }
}

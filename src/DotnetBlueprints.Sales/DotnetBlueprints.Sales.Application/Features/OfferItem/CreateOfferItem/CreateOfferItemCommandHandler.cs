using DotnetBlueprints.Sales.Application.Common.Interfaces;
using DotnetBlueprints.Sales.Domain.Entities;
using DotnetBlueprints.SharedKernel.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DotnetBlueprints.Sales.Application.Features.OfferItem.Create;

/// <summary>
/// Handles the creation of a new offer item.
/// </summary>
public class CreateOfferItemCommandHandler : IRequestHandler<CreateOfferItemCommand, Guid>
{
    private readonly ISalesDbContext _context;
    private readonly ILogger<CreateOfferItemCommandHandler> _logger;

    public CreateOfferItemCommandHandler(ISalesDbContext context, ILogger<CreateOfferItemCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Handles the process of creating a new offer item.
    /// </summary>
    /// <param name="request">The command containing offer item details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The unique identifier of the created offer item.</returns>
    public async Task<Guid> Handle(CreateOfferItemCommand request, CancellationToken cancellationToken)
    {
        var offer = await _context.Offers.FindAsync(new object[] { request.OfferId }, cancellationToken);
        if (offer == null)
        {
            throw new NotFoundException(nameof(Offer), request.OfferId);
        }

        var offerItem = new Domain.Entities.OfferItem(
            request.Name,
            request.Quantity,
            request.UnitPrice,
            request.OfferId
        );

        _context.OfferItems.Add(offerItem);
        await _context.SaveChangesAsync(cancellationToken);

        return offerItem.Id;
    }
}

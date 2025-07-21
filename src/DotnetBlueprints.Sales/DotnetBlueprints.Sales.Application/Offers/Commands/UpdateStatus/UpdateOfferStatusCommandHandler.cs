using DotnetBlueprints.Sales.Application.Common.Interfaces;
using DotnetBlueprints.Sales.Domain.Entities;
using DotnetBlueprints.SharedKernel.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DotnetBlueprints.Sales.Application.Offers.Commands.UpdateStatus;

public class UpdateOfferStatusCommandHandler : IRequestHandler<UpdateOfferStatusCommand, Unit>
{
    private readonly ISalesDbContext _context;
    private readonly ILogger<UpdateOfferStatusCommandHandler> _logger;

    public UpdateOfferStatusCommandHandler(ISalesDbContext context, ILogger<UpdateOfferStatusCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateOfferStatusCommand request, CancellationToken cancellationToken)
    {
        var offer = await _context.Offers.FindAsync(new object[] { request.OfferId }, cancellationToken);

        if (offer == null)
        {
            _logger.LogWarning("Offer not found: {OfferId}", request.OfferId);
            throw new NotFoundException(nameof(offer), request.OfferId);
        }

        try
        {
            offer.UpdateStatus(request.NewStatus);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid status transition from {FromStatus} to {ToStatus} for OfferId {OfferId}",
                offer.Status, request.NewStatus, request.OfferId);
            throw;
        }

        var history = OfferStatusHistory.Create(offer.Id, offer.Status, request.NewStatus, request.ChangedBy);
        _context.OfferStatusHistories.Add(history);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Offer {OfferId} status changed from {From} to {To}",
            offer.Id, offer.Status, request.NewStatus);

        return Unit.Value;
    }
}


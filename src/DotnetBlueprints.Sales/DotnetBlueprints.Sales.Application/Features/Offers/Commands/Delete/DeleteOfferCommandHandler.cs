using DotnetBlueprints.Sales.Application.Common.Interfaces;
using DotnetBlueprints.Sales.Domain.Entities;
using DotnetBlueprints.SharedKernel.Audit;
using DotnetBlueprints.SharedKernel.Exceptions;
using MediatR;
using System.Text.Json;

namespace DotnetBlueprints.Sales.Application.Features.Offers.Commands.Delete;

/// <summary>
/// Handles the logic for deleting (soft delete) an offer.
/// </summary>
public class DeleteOfferCommandHandler : IRequestHandler<DeleteOfferCommand, Unit>
{
    private readonly ISalesDbContext _context;
    private readonly IAuditHistoryRepository _auditRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteOfferCommandHandler"/>.
    /// </summary>
    /// <param name="context">The database context.</param>
    public DeleteOfferCommandHandler(ISalesDbContext context, IAuditHistoryRepository auditHistory)
    {
        _context = context;
        _auditRepository = auditHistory;
    }

    /// <summary>
    /// Handles the deletion of an offer by marking it as deleted.
    /// </summary>
    /// <param name="request">The delete offer command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task<Unit> Handle(DeleteOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await _context.Offers.FindAsync(new object[] { request.OfferId }, cancellationToken);
        if (offer == null || offer.IsDeleted)
            throw new NotFoundException("offer", nameof(offer));

        using var tx = await _context.BeginTransactionAsync(cancellationToken);

        var oldOffer = new Offer(offer.Title, offer.ValidUntil, offer.CreatedBy) { Id = offer.Id };

        offer.Delete(request.DeletedBy);

        var auditLogs = oldOffer.GetAuditChanges(offer, request.DeletedBy);
        foreach (var log in auditLogs)
        {
            _auditRepository.Add(log);
        }

        var outbox = new OutboxMessage(nameof(offer), JsonSerializer.Serialize(offer));

        _context.OutboxMessages.Add(outbox);
        await _context.SaveChangesAsync(cancellationToken);

        await tx.CommitAsync(cancellationToken);

        return Unit.Value;
    }
}

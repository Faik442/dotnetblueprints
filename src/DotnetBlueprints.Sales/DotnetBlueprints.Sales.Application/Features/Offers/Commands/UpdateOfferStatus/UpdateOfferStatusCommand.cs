using DotnetBlueprints.Sales.Application.Common.Interfaces;
using DotnetBlueprints.Sales.Domain.Entities;
using DotnetBlueprints.Sales.Domain.Enums;
using DotnetBlueprints.SharedKernel.Audit;
using DotnetBlueprints.SharedKernel.Exceptions;
using MediatR;
using System.Text.Json;

namespace DotnetBlueprints.Sales.Application.Features.Offers.Commands.UpdateStatus;

/// <summary>
/// Command to update the status of an existing offer.
/// </summary>
public record UpdateOfferStatusCommand(Guid OfferId, OfferStatus NewStatus, string ChangedBy) : IRequest<Unit>;

public class UpdateOfferStatusCommandHandler : IRequestHandler<UpdateOfferStatusCommand, Unit>
{
    private readonly ISalesDbContext _context;
    private readonly IAuditHistoryRepository _auditRepository;

    public UpdateOfferStatusCommandHandler(ISalesDbContext context, IAuditHistoryRepository auditRepository)
    {
        _context = context;
        _auditRepository = auditRepository;
    }

    /// <summary>
    /// Handles the offer status update command.
    /// </summary>
    /// <param name="request">The update command containing offer ID, new status, and changed by information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Unit value indicating completion.</returns>
    /// <exception cref="NotFoundException">Thrown when the offer is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the status transition is invalid.</exception>
    public async Task<Unit> Handle(UpdateOfferStatusCommand request, CancellationToken cancellationToken)
    {
        var offer = await _context.Offers.FindAsync(new object[] { request.OfferId }, cancellationToken);

        if (offer == null)
        {
            throw new NotFoundException(nameof(offer), request.OfferId);
        }

        using var tx = await _context.BeginTransactionAsync(cancellationToken);

        var oldOffer = new Offer(offer.UserId, offer.CompanyId, offer.Title, offer.ValidUntil, offer.CreatedBy) { Id = offer.Id };
        if (request.NewStatus != offer.Status)
        {
            offer.UpdateStatus(request.NewStatus);
        }
        else
        {
            throw new ValueAlreadySetException(nameof(offer), offer.Status);
        }

        var auditLogs = oldOffer.GetAuditChanges(offer, request.ChangedBy);
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

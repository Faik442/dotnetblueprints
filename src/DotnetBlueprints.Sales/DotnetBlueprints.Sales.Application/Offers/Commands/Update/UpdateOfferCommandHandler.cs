using DotnetBlueprints.Sales.Application.Common.Interfaces;
using DotnetBlueprints.Sales.Application.Events;
using DotnetBlueprints.Sales.Domain.Entities;
using DotnetBlueprints.Sales.Domain.Enums;
using DotnetBlueprints.SharedKernel.Audit;
using DotnetBlueprints.SharedKernel.Exceptions;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DotnetBlueprints.Sales.Application.Offers.Commands.Update;


/// <summary>
/// Handles the update of an existing offer, allowing changes to the title, validity date, and status.
/// </summary>
public class UpdateOfferCommandHandler : IRequestHandler<UpdateOfferCommand, Unit>
{
    private readonly ISalesDbContext _context;
    private readonly IAuditHistoryRepository _auditRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOfferCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public UpdateOfferCommandHandler(ISalesDbContext context, IAuditHistoryRepository auditRepository)
    {
        _context = context;
        _auditRepository = auditRepository;
    }

    /// <summary>
    /// Handles the update offer command.
    /// </summary>
    /// <param name="request">The update offer command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task<Unit> Handle(UpdateOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await _context.Offers.FindAsync(new object[] { request.OfferId }, cancellationToken);
        if (offer == null || offer.IsDeleted)
        {
            throw new NotFoundException(nameof(offer), request.OfferId);
        }

        using var tx = await _context.BeginTransactionAsync(cancellationToken);

        var oldOffer = new Offer(offer.Title, offer.ValidUntil, offer.CreatedBy) { Id = offer.Id };

        if (!string.IsNullOrWhiteSpace(request.Title))
            offer.UpdateTitle(request.Title);

        if (request.ValidUntil.HasValue)
            offer.UpdateValidUntil(request.ValidUntil.Value);

        if (request.Status.HasValue)
            offer.UpdateStatus((OfferStatus)request.Status.Value);

        offer.UpdatedBy = request.UpdatedBy;
        offer.UpdatedDate = DateTime.UtcNow;

        var auditLogs = oldOffer.GetAuditChanges(offer, request.UpdatedBy);
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
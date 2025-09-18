using DotnetBlueprints.Sales.Application.Common.Interfaces;
using DotnetBlueprints.Sales.Domain.Entities;
using DotnetBlueprints.Sales.Domain.Enums;
using DotnetBlueprints.SharedKernel.Audit;
using DotnetBlueprints.SharedKernel.Exceptions;
using MediatR;
using System.Text.Json;

namespace DotnetBlueprints.Sales.Application.Features.Offers.Commands.Update;

/// <summary>
/// Represents the request to update an existing offer’s title, validity date, or status.
/// </summary>
public class UpdateOfferCommand : IRequest<Unit>
{
    /// <summary>
    /// The unique identifier of the offer to update.
    /// </summary>
    public Guid OfferId { get; set; }

    /// <summary>
    /// The new title for the offer.
    /// </summary>
    public string? Title { get; set; } = string.Empty;

    /// <summary>
    /// The new validity date for the offer.
    /// </summary>
    public DateTime? ValidUntil { get; set; }

    /// <summary>
    /// The new status for the offer.
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// The user who is performing the update.
    /// </summary>
    public string UpdatedBy { get; set; } = string.Empty;
}

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

        var oldOffer = new Offer(offer.UserId, offer.CompanyId, offer.Title, offer.ValidUntil, offer.CreatedBy) { Id = offer.Id };

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
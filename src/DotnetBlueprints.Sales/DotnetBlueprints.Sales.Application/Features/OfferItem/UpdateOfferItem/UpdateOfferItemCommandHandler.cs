using DotnetBlueprints.Sales.Application.Common.Interfaces;
using DotnetBlueprints.Sales.Domain.Entities;
using DotnetBlueprints.SharedKernel.Audit;
using DotnetBlueprints.SharedKernel.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Sales.Application.Features.OfferItem.Update;

/// <summary>
/// Handles the update of an existing offer item.
/// </summary>
public class UpdateOfferItemCommandHandler : IRequestHandler<UpdateOfferItemCommand, Unit>
{
    private readonly ISalesDbContext _context;
    private readonly IAuditHistoryRepository _auditRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOfferItemCommandHandler"/> class.
    /// </summary>
    public UpdateOfferItemCommandHandler(ISalesDbContext context, IAuditHistoryRepository auditRepository)
    {
        _context = context;
        _auditRepository = auditRepository;
    }

    /// <summary>
    /// Handles the update offer item command.
    /// </summary>
    public async Task<Unit> Handle(UpdateOfferItemCommand request, CancellationToken cancellationToken)
    {
        var offerItem = await _context.OfferItems.FindAsync(new object[] { request.OfferItemId }, cancellationToken);

        if (offerItem == null)
            throw new NotFoundException(nameof(offerItem), request.OfferItemId);

        var oldOfferItem = new Domain.Entities.OfferItem(offerItem.Name, offerItem.Quantity, offerItem.UnitPrice, offerItem.OfferId) { Id = offerItem.Id };


        if (!string.IsNullOrWhiteSpace(request.Name))
            offerItem.UpdateDetails(request.Name, offerItem.UnitPrice);
        if (request.Quantity.HasValue)
            offerItem.UpdateQuantity(request.Quantity.Value);

        if (request.UnitPrice.HasValue)
            offerItem.UpdateDetails(offerItem.Name, request.UnitPrice.Value);

        offerItem.UpdatedBy = request.UpdatedBy;
        offerItem.UpdatedDate = DateTime.UtcNow;

        var auditLogs = oldOfferItem.GetAuditChanges(offerItem, request.UpdatedBy);
        foreach (var log in auditLogs)
            _auditRepository.Add(log);

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

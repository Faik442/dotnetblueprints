using DotnetBlueprints.Sales.Application.Common.Interfaces;
using DotnetBlueprints.SharedKernel.Exceptions;
using MediatR;

namespace DotnetBlueprints.Sales.Application.Features.OfferItem.Delete;

public class DeleteOfferItemCommandHandler : IRequestHandler<DeleteOfferItemCommand, Unit>
{
    private readonly ISalesDbContext _context;

    public DeleteOfferItemCommandHandler(ISalesDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteOfferItemCommand request, CancellationToken cancellationToken)
    {
        var offerItem = await _context.OfferItems.FindAsync(new object[] { request.OfferItemId }, cancellationToken);

        if (offerItem == null || offerItem.IsDeleted)
            throw new NotFoundException(nameof(offerItem), request.OfferItemId);

        offerItem.Delete(request.DeletedBy);

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

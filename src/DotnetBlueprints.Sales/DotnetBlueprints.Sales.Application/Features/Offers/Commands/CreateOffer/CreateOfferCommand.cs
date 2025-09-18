using DotnetBlueprints.Sales.Application.Common.Interfaces;
using DotnetBlueprints.Sales.Domain.Entities;
using MediatR;
using System.Text.Json;

namespace DotnetBlueprints.Sales.Application.Features.Offers.Commands.Create;

/// <summary>
/// Represents the request to create a new offer with one or more offer items.
/// </summary>
public class CreateOfferCommand : IRequest<Guid>
{
    /// <summary>
    /// The user of the offer.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// The company of the offer.
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// The title of the offer.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The validity date of the offer.
    /// </summary>
    public DateTime ValidUntil { get; set; }

    /// <summary>
    /// The user or process that creates the offer.
    /// </summary>
    public string? CreatedBy { get; set; }
}

public class CreateOfferCommandHandler : IRequestHandler<CreateOfferCommand, Guid>
{
    private readonly ISalesDbContext _context;
    public CreateOfferCommandHandler(ISalesDbContext context)
    {
        _context = context;
    }
    public async Task<Guid> Handle(CreateOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = new Offer(request.UserId, request.CompanyId, request.Title, request.ValidUntil, request.CreatedBy);

        using var tx = await _context.BeginTransactionAsync(cancellationToken);

        _context.Offers.Add(offer);

        var outbox = new OutboxMessage(nameof(Offer), JsonSerializer.Serialize(offer));

        _context.OutboxMessages.Add(outbox);
        await _context.SaveChangesAsync(cancellationToken);

        await tx.CommitAsync(cancellationToken);

        return offer.Id;
    }
}

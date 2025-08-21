using MediatR;

namespace DotnetBlueprints.Sales.Application.Features.OfferItem.Create;

/// <summary>
/// Command for creating a new offer item.
/// </summary>
public class CreateOfferItemCommand : IRequest<Guid>
{
    /// <summary>
    /// Gets or sets the identifier of the parent offer.
    /// </summary>
    public Guid OfferId { get; set; }

    /// <summary>
    /// Gets or sets the name or description of the item.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the item.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the item.
    /// </summary>
    public decimal UnitPrice { get; set; }
}

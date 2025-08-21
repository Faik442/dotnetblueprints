using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Sales.Application.OfferItem.Update;

using MediatR;
using System;

/// <summary>
/// Command to update an existing offer item.
/// </summary>
public class UpdateOfferItemCommand : IRequest<Unit>
{
    /// <summary>
    /// Gets or sets the identifier of the offer item to update.
    /// </summary>
    public Guid OfferItemId { get; set; }

    /// <summary>
    /// Gets or sets the new name or description for the item (optional).
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the new quantity for the item (optional).
    /// </summary>
    public int? Quantity { get; set; }

    /// <summary>
    /// Gets or sets the new unit price for the item (optional).
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the username or process that updated the offer item.
    /// </summary>
    public string UpdatedBy { get; set; } = string.Empty;
}


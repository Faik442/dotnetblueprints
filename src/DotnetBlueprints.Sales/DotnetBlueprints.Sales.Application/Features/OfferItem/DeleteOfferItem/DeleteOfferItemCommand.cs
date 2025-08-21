using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Sales.Application.Features.OfferItem.Delete;

public class DeleteOfferItemCommand : IRequest<Unit>
{
    /// <summary>
    /// Gets or sets the unique identifier of the offer item to delete.
    /// </summary>
    public Guid OfferItemId { get; set; }

    /// <summary>
    /// Gets or sets the user who deletes the offer item.
    /// </summary>
    public string DeletedBy { get; set; } = string.Empty;
}

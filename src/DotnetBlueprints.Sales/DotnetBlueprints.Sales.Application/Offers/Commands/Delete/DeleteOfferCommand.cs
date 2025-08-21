using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Sales.Application.Offers.Commands.Delete;

/// <summary>
/// Represents the command to delete (soft delete) an offer by its unique identifier.
/// </summary>
public class DeleteOfferCommand : IRequest<Unit>
{
    /// <summary>
    /// Gets or sets the unique identifier of the offer to delete.
    /// </summary>
    public Guid OfferId { get; set; }

    /// <summary>
    /// Gets or sets the username or process that requested the deletion.
    /// </summary>
    public string DeletedBy { get; set; } = string.Empty;
}

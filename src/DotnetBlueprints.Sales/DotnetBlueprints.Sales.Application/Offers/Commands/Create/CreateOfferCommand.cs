using DotnetBlueprints.Sales.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Sales.Application.Offers.Commands.Create;

/// <summary>
/// Represents the request to create a new offer with one or more offer items.
/// </summary>
public class CreateOfferCommand : IRequest<Guid>
{
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

using DotnetBlueprints.Sales.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Sales.Application.Offers.Queries.GetById;

/// <summary>
/// Query to retrieve an offer by its ID.
/// </summary>
public class GetOfferByIdQuery : IRequest<OfferDto>
{
    /// <summary>
    /// The unique identifier of the offer.
    /// </summary>
    public Guid OfferId { get; set; }

    public GetOfferByIdQuery(Guid offerId)
    {
        OfferId = offerId;
    }
}

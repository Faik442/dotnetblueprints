using DotnetBlueprints.Sales.Application.Common.Interfaces;
using DotnetBlueprints.Sales.Application.DTOs;
using DotnetBlueprints.SharedKernel.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Sales.Application.Offers.Queries.GetById;

/// <summary>
/// Handles retrieving an offer by ID.
/// </summary>
public class GetOfferByIdQueryHandler : IRequestHandler<GetOfferByIdQuery, OfferDto>
{
    private readonly ISalesDbContext _context;

    public GetOfferByIdQueryHandler(ISalesDbContext context)
    {
        _context = context;
    }

    public async Task<OfferDto> Handle(GetOfferByIdQuery request, CancellationToken cancellationToken)
    {
        var offer = await _context.Offers
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OfferId && !o.IsDeleted, cancellationToken);

        if (offer == null)
            throw new NotFoundException("Offer", request.OfferId);

        // Mapping entity to DTO (manual or with AutoMapper)
        return new OfferDto
        {
            Id = offer.Id,
            Title = offer.Title,
            ValidUntil = offer.ValidUntil,
            Status = offer.Status.ToString(),
            Items = offer.Items.Where(i => !i.IsDeleted).Select(i => new OfferItemDto
            {
                Id = i.Id,
                Name = i.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList(),
            TotalPrice = offer.TotalPrice
        };
    }
}
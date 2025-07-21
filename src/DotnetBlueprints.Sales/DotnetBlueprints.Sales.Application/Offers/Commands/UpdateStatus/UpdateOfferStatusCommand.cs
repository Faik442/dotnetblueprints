using DotnetBlueprints.Sales.Domain.Enums;
using MediatR;

namespace DotnetBlueprints.Sales.Application.Offers.Commands.UpdateStatus;

/// <summary>
/// Command to update the status of an existing offer.
/// </summary>
public record UpdateOfferStatusCommand(Guid OfferId, OfferStatus NewStatus, string ChangedBy) : IRequest<Unit>;

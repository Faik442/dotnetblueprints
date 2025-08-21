using MediatR;

namespace DotnetBlueprints.Sales.Application.Features.Offers.Commands.Update;

/// <summary>
/// Represents the request to update an existing offer’s title, validity date, or status.
/// </summary>
public class UpdateOfferCommand : IRequest<Unit>
{
    /// <summary>
    /// The unique identifier of the offer to update.
    /// </summary>
    public Guid OfferId { get; set; }

    /// <summary>
    /// The new title for the offer.
    /// </summary>
    public string? Title { get; set; } = string.Empty;

    /// <summary>
    /// The new validity date for the offer.
    /// </summary>
    public DateTime? ValidUntil { get; set; }

    /// <summary>
    /// The new status for the offer.
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// The user who is performing the update.
    /// </summary>
    public string UpdatedBy { get; set; } = string.Empty;
}
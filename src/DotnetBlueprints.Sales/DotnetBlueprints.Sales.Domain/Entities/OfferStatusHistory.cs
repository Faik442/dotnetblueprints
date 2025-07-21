using DotnetBlueprints.Sales.Domain.Enums;
using DotnetBlueprints.SharedKernel.Domain;

namespace DotnetBlueprints.Sales.Domain.Entities;

public class OfferStatusHistory : BaseEntity
{
    /// <summary>
    /// The ID of the offer whose status changed.
    /// </summary>
    public Guid OfferId { get; set; }

    /// <summary>
    /// The previous status of the offer.
    /// </summary>
    public OfferStatus FromStatus { get; set; }

    /// <summary>
    /// The new status of the offer.
    /// </summary>
    public OfferStatus ToStatus { get; set; }

    /// <summary>
    /// The date and time when the status change occurred.
    /// </summary>
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The user or system that triggered the change.
    /// </summary>
    public string ChangedBy { get; set; } = string.Empty;

    private OfferStatusHistory() { }

    /// <summary>
    /// Creates a new <see cref="OfferStatusHistory"/> record with the given status transition details.
    /// </summary>
    /// <param name="offerId">The unique identifier of the offer.</param>
    /// <param name="from">The previous status of the offer.</param>
    /// <param name="to">The new status the offer was changed to.</param>
    /// <param name="changedBy">The user or system that performed the status change.</param>
    /// <returns>A fully initialized <see cref="OfferStatusHistory"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="changedBy"/> is null or whitespace.</exception>
    public static OfferStatusHistory Create(Guid offerId, OfferStatus from, OfferStatus to, string changedBy)
    {
        if (string.IsNullOrWhiteSpace(changedBy))
            throw new ArgumentException("ChangedBy is required");

        return new OfferStatusHistory
        {
            OfferId = offerId,
            FromStatus = from,
            ToStatus = to,
            ChangedBy = changedBy,
            ChangedAt = DateTime.UtcNow
        };
    }
}


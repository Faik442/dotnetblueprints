using DotnetBlueprints.Sales.Domain.Enums;
using DotnetBlueprints.SharedKernel.Audit;
using DotnetBlueprints.SharedKernel.Domain;
using DotnetBlueprints.SharedKernel.Exceptions;

namespace DotnetBlueprints.Sales.Domain.Entities;

/// <summary>
/// Represents a commercial offer which contains a collection of offer items
/// and is valid until a specified expiration date.
/// </summary>
public class Offer : BaseEntity
{
    /// <summary>
    /// Gets the company of the offer.
    /// </summary>
    public Guid CompanyId { get; private set; }

    /// <summary>
    /// Gets the user of the offer.
    /// </summary>
    public Guid UserId { get; private set; }

    [Auditable]
    /// <summary>
    /// Gets the title or name of the offer.
    /// </summary>
    public string Title { get; private set; }

    [Auditable]
    /// <summary>
    /// Gets the date until which the offer is valid.
    /// </summary>
    public DateTime ValidUntil { get; private set; }

    [Auditable]
    /// <summary>
    /// Gets the current status of the offer, such as Draft, Sent, or Accepted.
    /// </summary>
    public OfferStatus Status { get; private set; } = OfferStatus.Draft;

    /// <summary>
    /// Gets the collection of items included in this offer.
    /// </summary>
    public ICollection<OfferItem> Items { get; private set; } = new List<OfferItem>();

    /// <summary>
    /// Gets the total price calculated from all offer items.
    /// </summary>
    public decimal TotalPrice => Items.Sum(i => i.Total);

    /// <summary>
    /// Initializes a new instance of the <see cref="Offer"/> class with a title and validity date.
    /// </summary>
    /// <param name="title">The title of the offer.</param>
    /// <param name="validUntil">The expiration date of the offer.</param>
    public Offer(Guid userId, Guid companyId, string title, DateTime validUntil, string? createdBy)
    {
        UserId = userId;
        CompanyId = companyId;
        Title = title;
        ValidUntil = validUntil;
        if (createdBy != null)
            CreatedBy = createdBy;
    }

    /// <summary>
    /// Adds an item to the offer with the specified name, quantity, and unit price.
    /// </summary>
    /// <param name="name">The name of the item.</param>
    /// <param name="quantity">The quantity of the item.</param>
    /// <param name="unitPrice">The price per unit of the item.</param>
    public void AddItem(string name, int quantity, decimal unitPrice)
    {
        Items.Add(new OfferItem(this, name, quantity, unitPrice));
    }

    /// <summary>
    /// Updates the title of the offer.
    /// </summary>
    /// <param name="title">The new title value.</param>
    /// <exception cref="ArgumentException">Thrown when the title is null or whitespace.</exception>
    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty");

        Title = title;
    }

    /// <summary>
    /// Updates the status of the offer.
    /// Status changes are restricted: once the offer is Sent, it cannot return to a previous state.
    /// </summary>
    /// <param name="status">The new status to assign.</param>
    /// <exception cref="InvalidOperationException">Thrown when attempting to regress from Sent to a previous state.</exception>
    public void UpdateStatus(OfferStatus status)
    {
        if (Status == OfferStatus.Sent && status == OfferStatus.Draft)
            throw new InvalidOperationException("Cannot revert an offer from Sent back to Draft.");

        Status = status;
    }

    /// <summary>
    /// Marks the offer as soft-deleted, recording the deletion details.
    /// </summary>
    /// <param name="deletedBy">The user or process that deleted the offer.</param>
    public void Delete(string deletedBy)
    {
        IsDeleted = true;
        DeletedDate = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Updates the validity date of the offer.
    /// </summary>
    /// <param name="validUntil">The new validity date.</param>
    public void UpdateValidUntil(DateTime validUntil)
    {
        if (validUntil <= DateTime.UtcNow)
            throw new ValidationException("ValidUntil must be a future date.");

        ValidUntil = validUntil;
    }
}
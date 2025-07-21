using DotnetBlueprints.Sales.Domain.Enums;
using DotnetBlueprints.SharedKernel.Domain;

namespace DotnetBlueprints.Sales.Domain.Entities;

/// <summary>
/// Represents a commercial offer which contains a collection of offer items
/// and is valid until a specified expiration date.
/// </summary>
public class Offer : BaseEntity
{
    /// <summary>
    /// Gets the title or name of the offer.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Gets the date until which the offer is valid.
    /// </summary>
    public DateTime ValidUntil { get; private set; }

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
    public Offer(string title, DateTime validUntil)
    {
        Title = title;
        ValidUntil = validUntil;
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
}
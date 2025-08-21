using DotnetBlueprints.SharedKernel.Audit;
using DotnetBlueprints.SharedKernel.Domain;
using DotnetBlueprints.SharedKernel.Exceptions;

namespace DotnetBlueprints.Sales.Domain.Entities;

/// <summary>
/// Represents an individual item within a commercial offer.
/// </summary>
public class OfferItem : BaseEntity
{
    [Auditable]
    /// <summary>
    /// Gets the name or description of the item.
    /// </summary>
    public string Name { get; private set; }

    [Auditable]
    /// <summary>
    /// Gets the quantity of the item included in the offer.
    /// </summary>
    public int Quantity { get; private set; }

    [Auditable]
    /// <summary>
    /// Gets the unit price of the item.
    /// </summary>
    public decimal UnitPrice { get; private set; }

    [Auditable]
    /// <summary>
    /// Gets the total price for this item, calculated as Quantity × UnitPrice.
    /// </summary>
    public decimal Total => Quantity * UnitPrice;

    /// <summary>
    /// Gets the identifier of the parent offer that this item belongs to.
    /// </summary>
    public Guid OfferId { get; private set; }

    /// <summary>
    /// Gets the parent <see cref="Offer"/> entity.
    /// </summary>
    public Offer Offer { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OfferItem"/> class.
    /// </summary>
    /// <param name="offer">The parent offer to which this item belongs.</param>
    /// <param name="name">The name or description of the item.</param>
    /// <param name="quantity">The quantity of the item. Must be greater than zero.</param>
    /// <param name="unitPrice">The unit price of the item. Must be greater than zero.</param>
    /// <exception cref="ValidationException">Thrown when the parent offer is null or quantity or unit price is invalid.</exception>
    public OfferItem(Offer offer, string name, int quantity, decimal unitPrice)
    {
        Offer = offer ?? throw new ArgumentNullException(nameof(offer));
        OfferId = offer.Id;

        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Name is required", nameof(name));

        if (quantity <= 0)
            throw new ValidationException("Quantity must be greater than zero", nameof(quantity));

        if (unitPrice <= 0)
            throw new ValidationException("UnitPrice must be greater than zero", nameof(unitPrice));

        Name = name;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public OfferItem(string name, int quantity, decimal unitPrice, Guid offerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Name is required", nameof(name));

        if (quantity <= 0)
            throw new ValidationException("Quantity must be greater than zero", nameof(quantity));

        if (unitPrice <= 0)
            throw new ValidationException("UnitPrice must be greater than zero", nameof(unitPrice));
        Name = name;
        Quantity = quantity;
        UnitPrice = unitPrice;
        OfferId = offerId;
    }

    /// <summary>
    /// Updates the quantity of the item.
    /// </summary>
    /// <param name="quantity">The new quantity value.</param>
    /// <exception cref="ValidationException">Thrown when the quantity is less than or equal to zero.</exception>
    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ValidationException("Quantity must be greater than zero.", nameof(quantity));

        Quantity = quantity;
    }

    /// <summary>
    /// Updates the name and unit price of the item.
    /// </summary>
    /// <param name="name">The new name. Must not be null or empty.</param>
    /// <param name="unitPrice">The new unit price. Must be greater than zero.</param>
    /// <exception cref="ValidationException">Thrown when name is empty or unit price is invalid.</exception>
    public void UpdateDetails(string name, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Name is required.", nameof(name));

        if (unitPrice <= 0)
            throw new ValidationException("UnitPrice must be greater than zero.", nameof(unitPrice));

        Name = name;
        UnitPrice = unitPrice;
    }

    /// <summary>
    /// Marks the offer as soft-deleted, recording the deletion details.
    /// </summary>
    /// <param name="deletedBy">The user or process that deleted the offer.</param>
    public void Delete(string deletedBy)
    {
        IsDeleted = true;
        DeletedDate = DateTime.Now;
        DeletedBy = deletedBy;
    }
}

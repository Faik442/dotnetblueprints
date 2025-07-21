namespace DotnetBlueprints.Sales.Domain.Enums;

/// <summary>
/// Represents the current lifecycle status of an offer.
/// </summary>
public enum OfferStatus
{
    /// <summary>
    /// The offer is created but not yet sent.
    /// </summary>
    Draft,

    /// <summary>
    /// The offer has been sent to the customer.
    /// </summary>
    Sent,

    /// <summary>
    /// The customer has accepted the offer.
    /// </summary>
    Accepted,

    /// <summary>
    /// The customer has rejected the offer.
    /// </summary>
    Rejected,

    /// <summary>
    /// The offer is no longer valid due to expiration.
    /// </summary>
    Expired
}
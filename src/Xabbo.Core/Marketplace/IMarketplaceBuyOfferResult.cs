namespace Xabbo.Core;

/// <summary>
/// The result of attempting to buy a marketplace offer.
/// </summary>
public interface IMarketplaceBuyOfferResult
{
    /// <summary>
    /// Gets the server result code for the purchase attempt.
    /// </summary>
    int Result { get; }
    /// <summary>
    /// Gets the ID of the offer that was bought.
    /// </summary>
    int OfferId { get; }
    /// <summary>
    /// Gets the offer's price at the time of purchase, if it changed since it was last seen.
    /// </summary>
    int NewPrice { get; }
    /// <summary>
    /// Gets the ID of the offer that was originally requested.
    /// </summary>
    int RequestedOfferId { get; }
}

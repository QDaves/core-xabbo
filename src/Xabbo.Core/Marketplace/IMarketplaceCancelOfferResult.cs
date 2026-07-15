namespace Xabbo.Core;

/// <summary>
/// The result of cancelling a marketplace offer.
/// </summary>
public interface IMarketplaceCancelOfferResult
{
    /// <summary>
    /// Gets the ID of the offer that was cancelled.
    /// </summary>
    int OfferId { get; }
    /// <summary>
    /// Gets whether the offer was successfully cancelled.
    /// </summary>
    bool Success { get; }
}

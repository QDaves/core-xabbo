using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// The result of cancelling all of the user's marketplace offers.
/// </summary>
public interface IMarketplaceCancelAllOffersResult
{
    /// <summary>
    /// Gets the IDs of the offers that were cancelled.
    /// </summary>
    IReadOnlyList<int> OfferIds { get; }
    /// <summary>
    /// Gets whether the offers were successfully cancelled.
    /// </summary>
    bool Success { get; }
}

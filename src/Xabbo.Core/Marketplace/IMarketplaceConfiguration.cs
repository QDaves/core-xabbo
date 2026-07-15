namespace Xabbo.Core;

/// <summary>
/// The server's current marketplace rules and limits.
/// </summary>
public interface IMarketplaceConfiguration
{
    /// <summary>
    /// Gets whether the marketplace is currently enabled.
    /// </summary>
    bool IsEnabled { get; }
    /// <summary>
    /// Gets the commission percentage taken on sales.
    /// </summary>
    int Commission { get; }
    /// <summary>
    /// Gets the credit price of a batch of marketplace tokens.
    /// </summary>
    int TokenBatchPrice { get; }
    /// <summary>
    /// Gets the number of tokens granted per batch purchase.
    /// </summary>
    int TokenBatchSize { get; }
    /// <summary>
    /// Gets the minimum allowed offer price in credits.
    /// </summary>
    int OfferMinPrice { get; }
    /// <summary>
    /// Gets the maximum allowed offer price in credits.
    /// </summary>
    int OfferMaxPrice { get; }
    /// <summary>
    /// Gets the number of hours an offer remains listed before expiring.
    /// </summary>
    int ExpirationHours { get; }
    /// <summary>
    /// Gets the period, in days, over which average sale prices are calculated.
    /// </summary>
    int AveragePricePeriod { get; }
    /// <summary>
    /// Gets the selling fee percentage.
    /// </summary>
    int SellingFeePercentage { get; }
    /// <summary>
    /// Gets the revenue limit.
    /// </summary>
    int RevenueLimit { get; }
    /// <summary>
    /// Gets the half-tax revenue threshold.
    /// </summary>
    int HalfTaxLimit { get; }
}

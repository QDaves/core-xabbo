using System;
using Xabbo.Messages;

namespace Xabbo.Core;

public interface IMarketplaceOffer : IItem, IComposable
{
    /// <summary>
    /// Gets the ID of the marketplace offer.
    /// </summary>
    new long Id { get; }
    /// <summary>
    /// Gets the current status of this marketplace offer.
    /// </summary>
    MarketplaceOfferStatus Status { get; }
    /// <summary>
    /// Gets the item data for the marketplace offer.
    /// </summary>
    IItemData Data { get; }
    /// <summary>
    /// Gets the price of the marketplace offer.
    /// </summary>
    int Price { get; }
    /// <summary>
    /// Gets the remaining time of this offer in minutes.
    /// </summary>
    int TimeRemaining { get; }
    /// <summary>
    /// Gets the average price for this item.
    /// </summary>
    int Average { get; }
    /// <summary>
    /// Gets the number of open offers for this item.
    /// Not available when loaded from the user's own marketplace offers.
    /// </summary>
    int Offers { get; }

    /// <summary>
    /// Gets whether this item supports being used/locked (e.g. a lovelock),
    /// showing who it was used by when <see cref="IsUsed"/> is <c>true</c>.
    /// </summary>
    bool IsUsable { get; }
    /// <summary>
    /// Gets whether this item has been used/locked. Only meaningful when <see cref="IsUsable"/> is <c>true</c>.
    /// </summary>
    bool IsUsed { get; }
    /// <summary>
    /// Gets the name of the user who used this item, if <see cref="IsUsed"/> is <c>true</c>.
    /// </summary>
    string? UsedByName { get; }
    /// <summary>
    /// Gets the name of the user this item was used with, if <see cref="IsUsed"/> is <c>true</c>.
    /// </summary>
    string? UsedWithName { get; }
    /// <summary>
    /// Gets the figure string of the user who used this item, if <see cref="IsUsed"/> is <c>true</c>.
    /// </summary>
    string? UsedByFigure { get; }
    /// <summary>
    /// Gets the figure string of the user this item was used with, if <see cref="IsUsed"/> is <c>true</c>.
    /// </summary>
    string? UsedWithFigure { get; }
    /// <summary>
    /// Gets the date this item was used, formatted as <c>dd/MM/yyyy</c>, if <see cref="IsUsed"/> is <c>true</c>.
    /// </summary>
    string? UsedDate { get; }
}

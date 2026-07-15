using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public class MarketplaceOffer : IMarketplaceOffer
{
    public long Id { get; set; }
    public MarketplaceOfferStatus Status { get; set; }
    public ItemType Type { get; set; }
    public int Kind { get; set; }
    public IItemData Data { get; set; }
    public int Price { get; set; }
    public int TimeRemaining { get; set; }
    public int Average { get; set; }
    public int Offers { get; set; }

    /// <summary>
    /// Gets whether this item supports being used/locked (e.g. a lovelock), in which
    /// case <see cref="IsUsed"/> indicates whether it already has been.
    /// </summary>
    public bool IsUsable { get; set; }
    /// <summary>
    /// Gets whether this item has been used/locked. Only meaningful when <see cref="IsUsable"/> is <c>true</c>.
    /// </summary>
    public bool IsUsed { get; set; }

    /// <summary>
    /// Best-effort extraction of who used this item, when <see cref="Data"/> is a
    /// <see cref="IStringArrayData"/> laid out like a lovelock's (index 1 = used-by
    /// name, 2 = used-with name, 3/4 = figures, 5 = date). Not guaranteed to apply to
    /// every usable item type; returns null if the shape doesn't match.
    /// </summary>
    public string? UsedByName => GetUsedString(1);
    public string? UsedWithName => GetUsedString(2);
    public string? UsedByFigure => GetUsedString(3);
    public string? UsedWithFigure => GetUsedString(4);
    public string? UsedDate => GetUsedString(5);

    private string? GetUsedString(int index)
        => Data is IStringArrayData array && index < array.Count ? array[index] : null;

    public MarketplaceOffer()
    {
        Data = new LegacyData();
    }

    protected MarketplaceOffer(IReadOnlyPacket packet, bool hasOfferCount)
    {
        Id = packet.ReadLegacyLong();
        Status = (MarketplaceOfferStatus)packet.ReadInt();

        int itemType = packet.ReadInt();
        switch (itemType)
        {
            case 1:
            case 4:
                // itemType 4 is a "usable" item (e.g. a lovelock) that can be used/locked.
                // It is otherwise identical to itemType 1: same polymorphic stuff data
                // (which is where a lovelock's used-by name/figure/date ends up, as a
                // StringArrayData), just followed by one extra raw boolean for IsUsed.
                Type = ItemType.Floor;
                Kind = packet.ReadInt();
                Data = ItemData.Parse(packet);
                if (itemType == 4)
                {
                    IsUsable = true;
                    IsUsed = packet.ReadBool();
                }
                break;
            case 2:
                Type = ItemType.Wall;
                Kind = packet.ReadInt();
                Data = new LegacyData() { Value = packet.ReadString() };
                break;
            case 3:
                Type = ItemType.Floor;
                Kind = packet.ReadInt();
                Data = new LegacyData()
                {
                    Flags = ItemDataFlags.IsLimitedRare,
                    UniqueSerialNumber = packet.ReadInt(),
                    UniqueSeriesSize = packet.ReadInt()
                };
                break;
            default: throw new Exception($"Unknown MarketplaceItem type: {itemType}");
        }

        Price = packet.ReadInt();
        TimeRemaining = packet.ReadInt();
        Average = packet.ReadInt();
        if (hasOfferCount)
            Offers = packet.ReadInt();
    }

    public void Compose(IPacket packet)
    {
        if (Data == null)
            throw new Exception("Data cannot be null");

        packet
            .WriteLegacyLong(Id)
            .WriteInt((int)Status);

        if (Type == ItemType.Floor)
        {
            if (Data.Flags.HasFlag(ItemDataFlags.IsLimitedRare))
            {
                packet
                    .WriteInt(3)
                    .WriteInt(Kind)
                    .WriteInt(Data.UniqueSerialNumber)
                    .WriteInt(Data.UniqueSeriesSize);
            }
            else
            {
                packet
                    .WriteInt(1)
                    .WriteInt(Kind)
                    .Write(Data);
            }
        }
        else if (Type == ItemType.Wall)
        {
            packet
                .WriteInt(2)
                .WriteInt(Kind)
                .WriteString(Data.Value);
        }
        else
        {
            throw new Exception($"Invalid MarketplaceItem type: {Type}");
        }

        packet
            .WriteInt(Price)
            .WriteInt(TimeRemaining)
            .WriteInt(Average);

        if (Offers > 0)
            packet.WriteInt(Offers);
    }

    public static MarketplaceOffer Parse(IReadOnlyPacket packet, bool hasOfferCount = true)
    { 
        return new MarketplaceOffer(packet, hasOfferCount);
    }

    public static IEnumerable<MarketplaceOffer> ParseMany(IReadOnlyPacket packet, bool hasOfferCount = true)
    {
        short n = packet.ReadLegacyShort();
        for (int i = 0; i < n; i++)
            yield return Parse(packet, hasOfferCount);
    }

    public override string ToString() => $"{nameof(MarketplaceOffer)}#{Id}/{Type}:{Kind}";
}

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

    public bool IsUsable { get; set; }
    public bool IsUsed { get; set; }
    public string? UsedByName { get; set; }
    public string? UsedWithName { get; set; }
    public string? UsedByFigure { get; set; }
    public string? UsedWithFigure { get; set; }
    public string? UsedDate { get; set; }

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
                Type = ItemType.Floor;
                Kind = packet.ReadInt();
                Data = ItemData.Parse(packet);
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
            case 4:
                // "Usable" items (e.g. lovelocks) that can be used/locked, showing who
                // used it. Structure verified against live search results; the extra
                // int read below (only present when IsUsable) has an unconfirmed meaning.
                Type = ItemType.Floor;
                Kind = packet.ReadInt();
                int usableFlag = packet.ReadInt();
                packet.ReadInt(); // constant marker, observed as 6 whenever usableFlag != 0
                IsUsable = usableFlag != 0;
                string usedState = packet.ReadString();
                Data = new LegacyData() { Value = usedState };
                if (IsUsable)
                {
                    IsUsed = usedState == "1";
                    UsedByName = packet.ReadString();
                    UsedWithName = packet.ReadString();
                    UsedByFigure = packet.ReadString();
                    UsedWithFigure = packet.ReadString();
                    UsedDate = packet.ReadString();
                }
                Offers = packet.ReadByte();
                TimeRemaining = packet.ReadInt();
                Price = packet.ReadInt();
                Average = packet.ReadInt();
                if (IsUsable)
                    packet.ReadInt();
                break;
            default: throw new Exception($"Unknown MarketplaceItem type: {itemType}");
        }

        if (itemType != 4)
        {
            Price = packet.ReadInt();
            TimeRemaining = packet.ReadInt();
            Average = packet.ReadInt();
            if (hasOfferCount)
                Offers = packet.ReadInt();
        }
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

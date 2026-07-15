using Xabbo.Messages;

namespace Xabbo.Core;

public class MarketplaceConfiguration : IMarketplaceConfiguration
{
    public bool IsEnabled { get; set; }
    public int Commission { get; set; }
    public int TokenBatchPrice { get; set; }
    public int TokenBatchSize { get; set; }
    public int OfferMinPrice { get; set; }
    public int OfferMaxPrice { get; set; }
    public int ExpirationHours { get; set; }
    public int AveragePricePeriod { get; set; }
    public int SellingFeePercentage { get; set; }
    public int RevenueLimit { get; set; }
    public int HalfTaxLimit { get; set; }

    protected MarketplaceConfiguration(IReadOnlyPacket packet)
    {
        IsEnabled = packet.ReadBool();
        Commission = packet.ReadInt();
        TokenBatchPrice = packet.ReadInt();
        TokenBatchSize = packet.ReadInt();
        OfferMinPrice = packet.ReadInt();
        OfferMaxPrice = packet.ReadInt();
        ExpirationHours = packet.ReadInt();
        AveragePricePeriod = packet.ReadInt();
        SellingFeePercentage = packet.ReadInt();
        RevenueLimit = packet.ReadInt();
        HalfTaxLimit = packet.ReadInt();
    }

    public static MarketplaceConfiguration Parse(IReadOnlyPacket packet) => new(packet);
}

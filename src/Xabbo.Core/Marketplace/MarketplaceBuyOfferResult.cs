using Xabbo.Messages;

namespace Xabbo.Core;

public class MarketplaceBuyOfferResult : IMarketplaceBuyOfferResult
{
    public int Result { get; set; }
    public int OfferId { get; set; }
    public int NewPrice { get; set; }
    public int RequestedOfferId { get; set; }

    protected MarketplaceBuyOfferResult(IReadOnlyPacket packet)
    {
        Result = packet.ReadInt();
        OfferId = packet.ReadInt();
        NewPrice = packet.ReadInt();
        RequestedOfferId = packet.ReadInt();
    }

    public static MarketplaceBuyOfferResult Parse(IReadOnlyPacket packet) => new(packet);
}

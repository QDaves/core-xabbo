using Xabbo.Messages;

namespace Xabbo.Core;

public class MarketplaceCancelOfferResult : IMarketplaceCancelOfferResult
{
    public int OfferId { get; set; }
    public bool Success { get; set; }

    protected MarketplaceCancelOfferResult(IReadOnlyPacket packet)
    {
        OfferId = packet.ReadInt();
        Success = packet.ReadBool();
    }

    public static MarketplaceCancelOfferResult Parse(IReadOnlyPacket packet) => new(packet);
}

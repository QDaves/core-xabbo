using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public class MarketplaceCancelAllOffersResult : IMarketplaceCancelAllOffersResult
{
    public List<int> OfferIds { get; set; }
    IReadOnlyList<int> IMarketplaceCancelAllOffersResult.OfferIds => OfferIds;
    public bool Success { get; set; }

    protected MarketplaceCancelAllOffersResult(IReadOnlyPacket packet)
    {
        int n = packet.ReadInt();
        OfferIds = new List<int>(n);
        for (int i = 0; i < n; i++)
            OfferIds.Add(packet.ReadInt());
        Success = packet.ReadBool();
    }

    public static MarketplaceCancelAllOffersResult Parse(IReadOnlyPacket packet) => new(packet);
}

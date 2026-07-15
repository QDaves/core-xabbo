using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class MakeMarketplaceOfferTask : InterceptorTask<int>
{
    private readonly ItemType _type;
    private readonly int _price;
    private readonly IEnumerable<long> _itemIds;

    public MakeMarketplaceOfferTask(IInterceptor interceptor, ItemType type, int price, IEnumerable<long> itemIds)
        : base(interceptor)
    {
        _type = type;
        _price = price;
        _itemIds = itemIds;
    }

    protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(
        Out.MarketplaceMakeOffer,
        _type switch
        {
            ItemType.Floor => 1,
            ItemType.Wall => 2,
            _ => throw new InvalidOperationException($"Invalid item type: {_type}.")
        },
        _price,
        _itemIds
    );

    [InterceptIn(nameof(Incoming.MarketplaceMakeOfferResult))]
    protected void HandleMarketplaceMakeOfferResult(InterceptArgs e)
    {
        e.Block();
        SetResult(e.Packet.ReadInt());
    }
}

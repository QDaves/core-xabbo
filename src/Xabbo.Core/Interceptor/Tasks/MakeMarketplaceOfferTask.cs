using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;
using ClientType = Xabbo.ClientType;

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
        _price,
        _type switch
        {
            ItemType.Floor => 1,
            ItemType.Wall => 2,
            _ => throw new InvalidOperationException($"Invalid item type: {_type}.")
        },
        _itemIds
    );

    [InterceptIn(nameof(Incoming.MarketplaceMakeOfferResult))]
    protected void HandleMarketplaceMakeOfferResult(InterceptArgs e)
    {
        e.Block();
        SetResult(e.Packet.ReadInt());
    }

    [InterceptIn(nameof(Incoming.ErrorReport))]
    protected void HandleErrorReport(InterceptArgs e)
    {
        int action = e.Packet.ReadInt();
        // The server reports errors keyed by the request's Unity header id, regardless of client type.
        if (action != Out.MarketplaceMakeOffer.GetValue(ClientType.Unity))
            return;

        e.Block();
        int errorCode = e.Packet.ReadInt();
        string timestamp = e.Packet.ReadString();
        SetException(new Exception($"Marketplace offer request was rejected (error code {errorCode} at {timestamp})."));
    }
}

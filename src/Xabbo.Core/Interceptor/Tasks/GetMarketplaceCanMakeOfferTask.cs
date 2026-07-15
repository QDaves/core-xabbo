using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class GetMarketplaceCanMakeOfferTask : InterceptorTask<(int ResultCode, int TokenCount)>
{
    public GetMarketplaceCanMakeOfferTask(IInterceptor interceptor)
        : base(interceptor) { }

    protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(Out.MarketplaceCanMakeOffer);

    [InterceptIn(nameof(Incoming.MarketplaceCanMakeOfferResult))]
    protected void HandleMarketplaceCanMakeOfferResult(InterceptArgs e)
    {
        e.Block();
        int resultCode = e.Packet.ReadInt();
        int tokenCount = e.Packet.ReadInt();
        SetResult((resultCode, tokenCount));
    }
}

using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class CancelAllMarketplaceOffersTask : InterceptorTask<IMarketplaceCancelAllOffersResult>
{
    public CancelAllMarketplaceOffersTask(IInterceptor interceptor)
        : base(interceptor) { }

    protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(Out.MarketplaceCancelAllOffers);

    [InterceptIn(nameof(Incoming.MarketplaceCancelAllOffersResult))]
    protected void HandleMarketplaceCancelAllOffersResult(InterceptArgs e)
    {
        e.Block();
        SetResult(MarketplaceCancelAllOffersResult.Parse(e.Packet));
    }
}

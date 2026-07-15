using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class CancelMarketplaceOfferTask : InterceptorTask<IMarketplaceCancelOfferResult>
{
    private readonly int _offerId;

    public CancelMarketplaceOfferTask(IInterceptor interceptor, int offerId)
        : base(interceptor)
    {
        _offerId = offerId;
    }

    protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(Out.MarketplaceCancelOffer, _offerId);

    [InterceptIn(nameof(Incoming.MarketplaceCancelOfferResult))]
    protected void HandleMarketplaceCancelOfferResult(InterceptArgs e)
    {
        var result = MarketplaceCancelOfferResult.Parse(e.Packet);
        if (result.OfferId == _offerId)
        {
            e.Block();
            SetResult(result);
        }
    }
}

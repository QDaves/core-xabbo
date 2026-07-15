using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class BuyMarketplaceOfferTask : InterceptorTask<IMarketplaceBuyOfferResult>
{
    private readonly int _offerId;

    public BuyMarketplaceOfferTask(IInterceptor interceptor, int offerId)
        : base(interceptor)
    {
        _offerId = offerId;
    }

    protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(Out.MarketplaceBuyOffer, _offerId);

    [InterceptIn(nameof(Incoming.MarketplaceBuyOfferResult))]
    protected void HandleMarketplaceBuyOfferResult(InterceptArgs e)
    {
        var result = MarketplaceBuyOfferResult.Parse(e.Packet);
        if (result.RequestedOfferId == _offerId)
        {
            e.Block();
            SetResult(result);
        }
    }
}

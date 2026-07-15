using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class SearchMarketplaceTask : InterceptorTask<IEnumerable<IMarketplaceOffer>>
{
    private readonly string? _searchText;
    private readonly int? _from, _to;
    private readonly MarketplaceSortOrder _sort;
    private readonly bool _combineLtds;

    public SearchMarketplaceTask(IInterceptor interceptor,
        string? searchText = null,
        int? from = null, int? to = null,
        MarketplaceSortOrder sort = MarketplaceSortOrder.HighestPrice,
        bool combineLtds = false)
        : base(interceptor)
    {
        _searchText = searchText;
        _from = from;
        _to = to;
        _sort = sort;
        _combineLtds = combineLtds;
    }

    protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(
        Out.MarketplaceSearchOffers,
        _from ?? -1, _to ?? -1,
        _searchText ?? string.Empty,
        (int)_sort,
        _combineLtds
    );

    [InterceptIn(nameof(Incoming.MarketplaceOpenOfferList))]
    protected void HandleMarketplaceOpenOfferList(InterceptArgs e)
    {
        e.Block();

        List<IMarketplaceOffer> offers = new();
        short n = e.Packet.ReadLegacyShort();
        try
        {
            for (int i = 0; i < n; i++)
                offers.Add(MarketplaceOffer.Parse(e.Packet));
        }
        catch (Exception ex)
        {
            // The server sent an offer layout we don't recognize (e.g. a new
            // marketplace item variant). The packet cursor can't be safely
            // resynced past it, so surface an error if nothing was parsed yet,
            // otherwise return what was successfully parsed before the failure
            // instead of hanging until timeout.
            if (offers.Count == 0)
            {
                SetException(new Exception(
                    $"Failed to parse marketplace offers (0/{n} parsed): {ex.Message}", ex));
                return;
            }
        }

        SetResult(offers);
    }
}

using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class GetMarketplaceConfigurationTask : InterceptorTask<IMarketplaceConfiguration>
{
    public GetMarketplaceConfigurationTask(IInterceptor interceptor)
        : base(interceptor) { }

    protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(Out.MarketplaceGetConfiguration);

    [InterceptIn(nameof(Incoming.MarketplaceConfiguration))]
    protected void HandleMarketplaceConfiguration(InterceptArgs e)
    {
        e.Block();
        SetResult(MarketplaceConfiguration.Parse(e.Packet));
    }
}

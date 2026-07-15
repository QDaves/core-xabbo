using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;
using Xabbo.Core.Events;
using Xabbo.Core.Game;

namespace Xabbo.Core.Tasks;

/// <summary>
/// Opens a trade with a user and waits for it to either open or fail to open.
/// Unlike the header-attribute based tasks, this binds directly to
/// <see cref="TradeManager"/>'s events since it already does the packet parsing.
/// </summary>
public class EnsureTradeTask : InterceptorTask<TradeStartEventArgs>
{
    private readonly TradeManager _tradeManager;
    private readonly int _userIndex;

    public EnsureTradeTask(IInterceptor interceptor, TradeManager tradeManager, int userIndex)
        : base(interceptor)
    {
        _tradeManager = tradeManager;
        _userIndex = userIndex;
    }

    protected override void OnBind()
    {
        _tradeManager.Opened += HandleOpened;
        _tradeManager.OpenFailed += HandleOpenFailed;
    }

    protected override void OnRelease()
    {
        _tradeManager.Opened -= HandleOpened;
        _tradeManager.OpenFailed -= HandleOpenFailed;
    }

    protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(Out.TradeOpen, _userIndex);

    private void HandleOpened(object? sender, TradeStartEventArgs e) => SetResult(e);

    private void HandleOpenFailed(object? sender, TradeStartFailEventArgs e)
        => SetException(new Exception($"Failed to open trade with '{e.Name}' (reason: {e.Reason})."));
}

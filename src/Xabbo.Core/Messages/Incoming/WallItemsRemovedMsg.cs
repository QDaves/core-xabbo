using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when multiple wall items are removed from the room at once, e.g. by wired.
/// <para/>
/// Supported clients: <see cref="ClientType.Flash"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.ItemRemoveMultiple"/></item>
/// </list>
/// </summary>
public sealed class WallItemsRemovedMsg : List<Id>, IMessage<WallItemsRemovedMsg>
{
    public WallItemsRemovedMsg() { }
    public WallItemsRemovedMsg(int capacity) : base(capacity) { }

    public static Identifier Identifier => In.ItemRemoveMultiple;

    static WallItemsRemovedMsg IParser<WallItemsRemovedMsg>.Parse(in PacketReader p)
    {
        int n = p.ReadLength();
        var msg = new WallItemsRemovedMsg(n);
        for (int i = 0; i < n; i++)
            msg.Add(p.ReadId());

        if (p.Available >= 4)
            p.ReadInt();

        return msg;
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteLength((Length)Count);
        foreach (Id id in this)
            p.WriteId(id);
        p.WriteInt(-1);
    }
}

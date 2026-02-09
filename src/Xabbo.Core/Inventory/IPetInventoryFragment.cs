using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public interface IPetInventoryFragment : IReadOnlyCollection<IInventoryPet>
{
    /// <summary>
    /// Gets the total number of fragments.
    /// </summary>
    int Total { get; }

    /// <summary>
    /// Gets the index of this fragment.
    /// </summary>
    int Index { get; }
}

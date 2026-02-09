using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xabbo.Messages;
using Xabbo.Interceptor;

using Xabbo.Core.Events;
using Xabbo.Extension;

namespace Xabbo.Core.Game;

/// <summary>
/// Manages the user's inventory.
/// </summary>
public class PetInventoryManager : GameStateManager
{
    private readonly ILogger _logger;

    private readonly List<PetInventoryFragment> _fragments = new();
    private bool _forceLoadingInventory;
    private int _currentPacketIndex;
    private int _totalPackets;

    private TaskCompletionSource<IPetInventory> _loadTcs
        = new TaskCompletionSource<IPetInventory>(TaskCreationOptions.RunContinuationsAsynchronously);

    private PetInventory? _inventory;
    public IPetInventory? Inventory => _inventory;

    public event EventHandler? Invalidated;
    protected virtual void OnInvalidated()
        => Invalidated?.Invoke(this, EventArgs.Empty);

    public event EventHandler? Loaded;
    protected virtual void OnLoaded()
        => Loaded?.Invoke(this, EventArgs.Empty);

    public event EventHandler<InventoryPetEventArgs>? PetAdded;
    protected virtual void OnPetAdded(InventoryPet item)
        => PetAdded?.Invoke(this, new InventoryPetEventArgs(item));

    public event EventHandler<InventoryPetEventArgs>? PetUpdated;
    protected virtual void OnPetUpdated(InventoryPet item)
        => PetUpdated?.Invoke(this, new InventoryPetEventArgs(item));

    public event EventHandler<InventoryPetEventArgs>? PetRemoved;
    protected virtual void OnPetRemoved(InventoryPet item)
        => PetRemoved?.Invoke(this, new InventoryPetEventArgs(item));

    public PetInventoryManager(ILogger<PetInventoryManager> logger, IExtension extension)
        : base(extension)
    {
        _logger = logger;
    }

    public PetInventoryManager(IExtension extension)
        : base(extension)
    {
        _logger = NullLogger.Instance;
    }

    protected override void OnDisconnected(object? sender, EventArgs e)
    {
        base.OnDisconnected(sender, e);

        _inventory = null;
        _forceLoadingInventory = false;
        _currentPacketIndex = 0;
        _totalPackets = 0;
    }

    /// <summary>
    /// Returns the inventory immediately if it is available
    /// and has not been invalidated, otherwise attempts to retrieve it from the server.
    /// Note that the user must be in a room to retrieve the inventory from the server.
    /// If the user is not in a room and a request to load the inventory is made, this method will time out.
    /// </summary>
    public async Task<IPetInventory> GetPetInventoryAsync(int timeout = XabboConst.DefaultTimeout,
        CancellationToken cancellationToken = default)
    {
        if (_inventory?.IsInvalidated == false)
        {
            return _inventory;
        }
        else
        {
            Task<IPetInventory> loadTask = _loadTcs.Task;

            CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            if (timeout > 0) cts.CancelAfter(timeout);

            try
            {
                if (!_forceLoadingInventory)
                {
                    await Extension.SendAsync(Out.GetPetInventory);
                    _forceLoadingInventory = true;
                }
                
                await await Task.WhenAny(loadTask, Task.Delay(Timeout.Infinite, cts.Token));
                return await loadTask;
            }
            finally { cts.Dispose(); }
        }
    }

    private void SetLoadTaskResult(IPetInventory inventory)
    {
        _loadTcs.TrySetResult(inventory);
        _loadTcs = new TaskCompletionSource<IPetInventory>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    [InterceptIn(nameof(Incoming.PetInventory))]
    protected void HandlePetInventory(InterceptArgs e)
    {
        if (_forceLoadingInventory) e.Block();

        PetInventoryFragment fragment = PetInventoryFragment.Parse(e.Packet);

        if (fragment.Index == 0)
        {
            _logger.LogTrace("Resetting pet inventory load state.");

            _currentPacketIndex = 0;
            _totalPackets = fragment.Total;
            _fragments.Clear();
        }
        else if (_currentPacketIndex != fragment.Index ||
            _totalPackets != fragment.Total)
        {
            _logger.LogWarning(
                "Pet inventory load state mismatch!"
                + " Expected {expectedIndex}/{expectedTotal};"
                + " received {actualIndex}/{actualTotal} (index/total).",
                _currentPacketIndex, _totalPackets,
                fragment.Index, fragment.Total
            );
            return;
        }

        _logger.LogTrace("Received pet inventory fragment {n} of {total}.", fragment.Index + 1, fragment.Total);

        _currentPacketIndex++;
        _fragments.Add(fragment);

        if (fragment.Index == (fragment.Total - 1))
        {
            _logger.LogTrace("All pet inventory fragments received.");

            _inventory ??= new PetInventory();
            _inventory.Clear();
            _inventory.IsInvalidated = false;

            foreach (InventoryPet item in _fragments.SelectMany(fragment => (ICollection<InventoryPet>)fragment))
            {
                if (!_inventory.TryAdd(item))
                {
                    _logger.LogWarning("Failed to add pet inventory item {Id}!", item.Id);
                }
            }

            _forceLoadingInventory = false;

            SetLoadTaskResult(_inventory);
            OnLoaded();
        }
    }

    [InterceptIn(nameof(Incoming.PetAddedToInventory))]
    protected virtual void HandlePetAddedToInventory(InterceptArgs e)
    {
        if (_inventory is null) return;

        var item = InventoryPet.Parse(e.Packet);
        _inventory.AddOrUpdate(item, out bool added);

        if (added)
        {
            _logger.LogTrace("Added inventory item {id}.", item.Id);
            OnPetAdded(item);
        }
        else
        {
            _logger.LogTrace("Updated inventory item {id}.", item.Id);
            OnPetUpdated(item);
        }
    }

    [InterceptIn(nameof(Incoming.PetRemovedFromInventory))]
    protected virtual void HandlePetRemovedFromInventory(InterceptArgs e)
    {
        if (_inventory is null) return;

        long itemId = e.Packet.ReadLegacyLong();
        if (_inventory.TryRemove(itemId, out InventoryPet? item))
        {
            _logger.LogTrace("Pet inventory item {id} removed.", itemId);
            OnPetRemoved(item);
        }
        else
        {
            _logger.LogWarning("Failed to find pet inventory item {id} to remove!", itemId);
        }
    }
}

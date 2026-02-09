using System;
using System.Linq;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Xabbo.Core.Game;

public class PetInventory : IPetInventory, IEnumerable<InventoryPet>
{
    private readonly ConcurrentDictionary<long, InventoryPet> _items = new();

    public bool IsInvalidated { get; set; }

    public PetInventory() { }

    public PetInventory(IEnumerable<InventoryPet> items)
    {
        foreach (InventoryPet item in items)
        {
            TryAdd(item);
        }
    }

    public bool TryAdd(InventoryPet item) => _items.TryAdd(item.Id, item);

    public InventoryPet AddOrUpdate(InventoryPet item, out bool added)
    {
        return _items.AddOrUpdate(item.Id, item, (id, existingItem) => item, out added);
    }

    public InventoryPet? GetItem(long id) => TryGetItem(id, out InventoryPet? item) ? item : null;
    IInventoryPet? IPetInventory.GetItem(long id) => GetItem(id);
    public bool TryGetItem(long itemId, [NotNullWhen(true)] out InventoryPet? item) => _items.TryGetValue(itemId, out item);
    bool IPetInventory.TryGetItem(long id, [NotNullWhen(true)] out IInventoryPet? item) => (item = GetItem(id)) is not null;

    public bool TryRemove(long itemId, [NotNullWhen(true)] out InventoryPet? item) => _items.TryRemove(itemId, out item);
    public void Clear() => _items.Clear();

    public IEnumerator<InventoryPet> GetEnumerator() => _items.Select(x => x.Value).GetEnumerator();
    IEnumerator<IInventoryPet> IEnumerable<IInventoryPet>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

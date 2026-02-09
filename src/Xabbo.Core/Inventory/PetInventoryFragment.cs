using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public class PetInventoryFragment : IPetInventoryFragment, ICollection<InventoryPet>
{
    private readonly List<InventoryPet> _list = new List<InventoryPet>();

    public int Total { get; set; }
    public int Index { get; set; }

    bool ICollection<InventoryPet>.IsReadOnly => false;

    public int Count => _list.Count;

    public InventoryPet this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    public PetInventoryFragment() { }

    public PetInventoryFragment(IEnumerable<IInventoryPet> items)
    {
        //_list.AddRange(items.Select(
            //item => item is InventoryPet inventoryItem ? inventoryItem : new InventoryPet(item)
        //));
        _list.AddRange(items.Select(x => (InventoryPet)x).ToList());
    }

    protected PetInventoryFragment(IReadOnlyPacket packet)
    {
        Total = packet.ReadInt();
        Index = packet.ReadInt();

        _list.AddRange(InventoryPet.ParseMany(packet));
    }

    public void Add(InventoryPet item) => _list.Add(item);
    public bool Remove(InventoryPet item) => _list.Remove(item);
    public void Clear() => _list.Clear();
    public bool Contains(InventoryPet item) => _list.Contains(item);
    public void CopyTo(InventoryPet[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

    public IEnumerator<InventoryPet> GetEnumerator() => _list.GetEnumerator();
    IEnumerator<IInventoryPet> IEnumerable<IInventoryPet>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static PetInventoryFragment Parse(IReadOnlyPacket packet) => new PetInventoryFragment(packet);
}

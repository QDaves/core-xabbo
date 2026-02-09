using System;

namespace Xabbo.Core.Events;

public class InventoryPetEventArgs : EventArgs
{
    public IInventoryPet Pet { get; }
    public InventoryPetEventArgs(IInventoryPet pet)
    {
        Pet = pet;
    }
}

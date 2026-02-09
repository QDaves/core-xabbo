using System;
using System.Collections.Generic;
using Xabbo.Messages;

namespace Xabbo.Core;

public interface IInventoryPet
{
    /// <summary>
    /// Gets the ID of the pet.
    /// </summary>
    long Id { get; }
    /// <summary>
    /// Gets the name of the pet.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Gets the type ID of the pet.
    /// </summary>
    int TypeId { get; }
    /// <summary>
    /// Gets the palette ID of the pet.
    /// </summary>
    int PaletteId { get; }
    /// <summary>
    /// Gets the color value of the pet.
    /// </summary>
    string Color { get; }
    /// <summary>
    /// Gets the breed ID of the pet.
    /// </summary>
    int BreedId { get; }
    /// <summary>
    /// Gets the custom part data of the pet.
    /// </summary>
    List<int[]> CustomParts { get; }
    /// <summary>
    /// Gets the level of the pet.
    /// </summary>
    int Level { get; }
}

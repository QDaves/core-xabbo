using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabbo.Messages;

namespace Xabbo.Core;
public class InventoryPet : IInventoryPet
{
    public long Id { get; set; }
    public string Name { get; set; }

    public int TypeId { get; set; }
    public int PaletteId { get; set; }
    public string Color { get; set; }
    public int BreedId { get; set; }
    public List<int[]> CustomParts { get; set; } = new();

    public int Level { get; set; }

    protected InventoryPet(IReadOnlyPacket p)
    {
        Id = p.ReadLegacyLong();
        Name = p.ReadString();

        TypeId = p.ReadInt();
        PaletteId = p.ReadInt();
        Color = p.ReadString();
        BreedId = p.ReadInt();
        
        var customPartsCount = p.ReadInt();
        for (var i = 0; i < customPartsCount; i++)
        {
            CustomParts.Add(new int[] { p.ReadInt(), p.ReadInt(), p.ReadInt() });
        }

        Level = p.ReadInt();
    }

    public static InventoryPet Parse(IReadOnlyPacket packet) => new InventoryPet(packet);

    public static IEnumerable<InventoryPet> ParseMany(IReadOnlyPacket packet)
    {
        short n = packet.ReadLegacyShort();
        for (int i = 0; i < n; i++)
        {
            yield return Parse(packet);
        }
    }
}

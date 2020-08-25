using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GvasFormat.Serialization.UETypes
{
    class UESoftObjectProperty : UEProperty
    {

        public UESoftObjectProperty(BinaryReader reader, long valueLength)
        {
            int terminator = reader.ReadByte();
            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            int unknown = reader.ReadInt16();

            terminator = reader.ReadByte();
            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            Model = reader.ReadUEString();
        }

        public override void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public string Model;
    }
}

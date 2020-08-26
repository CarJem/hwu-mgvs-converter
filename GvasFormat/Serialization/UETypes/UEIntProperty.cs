using System;
using System.Diagnostics;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEIntProperty : UEProperty
    {
        public UEIntProperty() { }
        public UEIntProperty(BinaryReader reader, long valueLength)
        {
            var terminator = reader.ReadByte();
            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            if (valueLength != sizeof(int))
                throw new FormatException($"Expected int value of length {sizeof(int)}, but was {valueLength}");

            Value = reader.ReadInt32();
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.WriteUEString(Name);
            writer.WriteUEString(Type);
            writer.WriteInt64(sizeof(int)); //valueLength
            writer.Write(false); //terminator
            writer.WriteInt32(Value);
        }

        public int Value;
    }
}
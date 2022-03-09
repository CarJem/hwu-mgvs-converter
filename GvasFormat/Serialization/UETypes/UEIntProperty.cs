using System;
using System.Diagnostics;
using System.IO;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEIntProperty : UEProperty
    {
        public UEIntProperty() { }
        public UEIntProperty(BinaryReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            if (valueLength == -1)
            {//IntProperty in MapProperty
                Value = reader.ReadInt32();
                return;
            }
            var terminator = reader.ReadByte();

            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            if (valueLength != sizeof(int))
                throw new FormatException($"Expected int value of length {sizeof(int)}, but was {valueLength}");

            Value = reader.ReadInt32();
        }

        public override void SerializeProp(BinaryWriter writer)
        {
            if (ValueLength != -1)
                writer.Write(false); //terminator
            writer.WriteInt32(Value);
        }

        public int Value;
    }
}
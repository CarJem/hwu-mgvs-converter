using System;
using System.Diagnostics;
using System.IO;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEFloatProperty : UEProperty
    {
        public UEFloatProperty() { }
        public UEFloatProperty(GvasReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            var terminator = reader.ReadByte();
            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            if (valueLength != sizeof(float))
                throw new FormatException($"Expected float value of length {sizeof(float)}, but was {valueLength}");

            Value = reader.ReadSingle();
        }

        public override long SerializeProp(GvasWriter writer)
        {
            long size = 0;
            writer.Write(false); //terminator
            size += writer.WriteSingle(Value);
            return size;
        }

        public float Value;
    }
}
using System;
using System.Diagnostics;
using System.IO;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEDoubleProperty : UEProperty
    {
        public UEDoubleProperty() { }
        public UEDoubleProperty(GvasReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            if (valueLength == -1)
            {
                Value = reader.ReadInt32();
                return;
            }
            var terminator = reader.ReadByte();

            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            if (valueLength != sizeof(double))
                throw new FormatException($"Expected double value of length {sizeof(double)}, but was {valueLength}");

            Value = reader.ReadDouble();
        }

        public override long SerializeProp(GvasWriter writer)
        {
            long size = 0;

            if (!Indexed) writer.Write(false); //terminator
            size += writer.WriteDouble(Value);

            return size;
        }

        public double Value;
    }
}
using System;
using System.Diagnostics;
using System.IO;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEInt64Property : UEProperty
    {
        public UEInt64Property() { }
        public UEInt64Property(GvasReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            if (valueLength == -1) {//Int64Property in MapProperty
                Value = reader.ReadInt32();
                return;
            }
            var terminator = reader.ReadByte();

            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            if (valueLength != sizeof(Int64))
                throw new FormatException($"Expected double value of length {sizeof(Int64)}, but was {valueLength}");

            Value = reader.ReadInt64();
        }

        public override long SerializeProp(GvasWriter writer)
        {
            long size = 0;
            if (!Indexed) writer.Write(false); //terminator
            size += writer.WriteInt64(Value);
            return size;
        }

        public Int64 Value;
    }
}
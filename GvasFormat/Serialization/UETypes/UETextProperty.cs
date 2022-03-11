using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UETextProperty : UEProperty
    {
        private static readonly Encoding Utf8 = new UTF8Encoding(false);

        public UETextProperty() { }
        public UETextProperty(GvasReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            var terminator = reader.ReadByte();
            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            // valueLength starts here
            Flags = reader.ReadInt64();
/*
            if (Flags != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 8:x8}. Expected text ??? {0x00}, but was {Flags:x16}");
*/

            //terminator = reader.ReadByte();
            //if (terminator != 0)
            //    throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            Id = reader.ReadByte().ToString();
            Value = reader.ReadUEString();
        }

        public override long SerializeProp(GvasWriter writer)
        {
            long size = 0;
            writer.Write(false); //terminator
            size += writer.WriteInt64(Flags);
            size += writer.Write(Byte.Parse(Id));
            size += writer.WriteUEString(Value);
            return size;
        }

        public long Flags;
        public string Id;
        public string Value;
    }
}
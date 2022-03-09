using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEStringProperty : UEProperty
    {
        public UEStringProperty() { }
        public UEStringProperty(BinaryReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            if (valueLength > -1)
            {
                var terminator = reader.ReadByte();
                if (terminator != 0)
                    throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");
                Value = reader.ReadUEString(valueLength);
            } else
            {
                Value = reader.ReadUEString();
            }

        }

        public override void SerializeProp(BinaryWriter writer)
        {
            if (ValueLength != -1)
                writer.Write(false); //terminator
            writer.WriteUEString(Value, ValueLength);
        }

        public string Value;
    }
}
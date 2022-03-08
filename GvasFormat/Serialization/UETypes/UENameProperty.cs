﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UENameProperty : UEProperty
    {
        private static readonly Encoding Utf8 = new UTF8Encoding(false);

        public UENameProperty() { }
        public UENameProperty(BinaryReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            if (valueLength > -1)
            {
                var terminator = reader.ReadByte();
                if (terminator != 0)
                    throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");
                Value = reader.ReadUEStringProperty(valueLength);
            } else
            {
                Value = reader.ReadUEString();
            }

        }
        public override void SerializeMap(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void SerializeProp(BinaryWriter writer, bool bodyOnly = false)
        {
            if (!bodyOnly) writer.Write(false); //terminator
            writer.WriteUEString(Value);
        }

        public override void SerializeProp(BinaryWriter writer)
        {
            SerializeProp(writer, false);
        }

        public string Value;
    }
}
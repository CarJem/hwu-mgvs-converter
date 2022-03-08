﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using GvasFormat.Utils;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEByteProperty : UEProperty
    {
        private static readonly Encoding Utf8 = new UTF8Encoding(false);

        public UEByteProperty() { }
        public static UEByteProperty Read(BinaryReader reader, string name, string type, long valueLength)
        {
            var terminator = reader.ReadTerminator();
            var arrayLength = reader.ReadInt32();
            var bytes = reader.ReadBytes(arrayLength);
            return new UEByteProperty {Value = bytes, Name = name, Type = type, ValueLength = valueLength };
        }
        public override void SerializeMap(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void SerializeProp(BinaryWriter writer)
        {
            writer.Write(false); //terminator
            writer.WriteInt32(Value.Length);
            writer.Write(Value);
        }

        public byte[] Value;
    }
}
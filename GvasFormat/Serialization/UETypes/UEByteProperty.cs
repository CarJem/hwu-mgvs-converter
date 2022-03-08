using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using GvasFormat.Utils;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEByteProperty : UEProperty
    {
        public UEByteProperty() { }
        public UEByteProperty(BinaryReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            var terminator = reader.ReadTerminator();
            var arrayLength = reader.ReadInt32();
            Value = reader.ReadBytes(arrayLength);
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
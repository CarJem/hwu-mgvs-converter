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
        public UEByteProperty(GvasReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            if (valueLength > -1) reader.ReadTerminator();
            var arrayLength = reader.ReadInt32();
            Value = reader.ReadBytes(arrayLength);
        }

        public override long SerializeProp(GvasWriter writer)
        {
            long size = 0;
            if (!Indexed) size += writer.Write(false); //terminator
            size += writer.WriteInt32(Value.Length);
            size += writer.Write(Value);

            return size;
        }

        public byte[] Value;
    }
}
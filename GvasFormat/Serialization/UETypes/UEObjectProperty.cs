using GvasFormat.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEObjectProperty : UEProperty
    {
        public UEObjectProperty() { }
        public UEObjectProperty(GvasReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            var terminator = reader.ReadByte();
            Value = reader.ReadBytes((int)valueLength);
        }

        public override long SerializeProp(GvasWriter writer)
        {
            long size = 0;
            writer.Write(false); //terminator
            size += writer.Write(Value);
            return size;
        }

        public byte[] Value;
    }
}
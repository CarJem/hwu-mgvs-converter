using GvasFormat.Utils;
using System;
using System.Diagnostics;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEGuidStructProperty : UEStructProperty
    {
        public UEGuidStructProperty() { }

        public UEGuidStructProperty(GvasReader reader, string name, string type, string structType, long valueLength) : base(name, type, structType, valueLength)
        {
            Value = new Guid(reader.ReadBytes(16));
        }

        public Guid Value;

        public override long SerializeStructProp(GvasWriter writer)
        {
            long size = 0;
            size += writer.Write(Value.ToByteArray());
            return size;
        }
    }
}
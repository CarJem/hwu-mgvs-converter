using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEDateTimeStructProperty : UEStructProperty
    {
        public UEDateTimeStructProperty() { }

        public UEDateTimeStructProperty(GvasReader reader, string name, string type, string structType, long valueLength) : base(name, type, structType, valueLength)
        {
            Value = DateTime.FromBinary(reader.ReadInt64());
        }

        public DateTime Value;

        public override long SerializeStructProp(GvasWriter writer)
        {
            long size = 0;
            size += writer.WriteInt64(Value.ToBinary());
            return size;
        }
    }
}
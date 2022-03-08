using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEDateTimeStructProperty : UEStructProperty
    {
        public UEDateTimeStructProperty() { }

        public UEDateTimeStructProperty(BinaryReader reader, string name, string type, string structType, long valueLength)
        {
            Name = name;
            Type = type;
            StructType = structType;
            ValueLength = valueLength;

            Value = DateTime.FromBinary(reader.ReadInt64());
        }

        public DateTime Value;

        public override void SerializeStructProp(BinaryWriter writer)
        {
            writer.WriteInt64(Value.ToBinary());
        }
    }
}
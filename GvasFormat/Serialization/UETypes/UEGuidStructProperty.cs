using System;
using System.Diagnostics;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEGuidStructProperty : UEStructProperty
    {
        public UEGuidStructProperty() { }

        public UEGuidStructProperty(BinaryReader reader)
        {
            Value = new Guid(reader.ReadBytes(16));
        }

        public Guid Value;

        public override void Serialize(BinaryWriter writer)
        {
            writer.WriteUEString(Name);
            writer.WriteUEString(Type);
            writer.WriteInt64(0); //valueLength
            writer.WriteUEString(StructType);
            writer.Write(Value.ToByteArray());
        }
    }
}
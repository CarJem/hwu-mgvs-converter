using System;
using System.Diagnostics;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("", Name = "{Name}")]
    public sealed class UENoneProperty : UEProperty
    {
        public const string PropertyName = "None";
        public UENoneProperty() { Name = PropertyName; }

        public override void SerializeProp(BinaryWriter writer)
        {
            long pos = writer.BaseStream.Position;
            writer.WriteUEString(UENoneProperty.PropertyName);
        }
    }
}
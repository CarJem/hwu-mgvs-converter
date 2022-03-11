using System;
using System.Diagnostics;
using System.IO;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("", Name = "{Name}")]
    public sealed class UENoneProperty : UEProperty
    {
        public const string PropertyName = "None";
        public UENoneProperty() { Name = PropertyName; }

        public override long SerializeProp(GvasWriter writer)
        {
            long size = 0;
            size += writer.WriteUEString(UENoneProperty.PropertyName);
            return size;
        }
    }
}
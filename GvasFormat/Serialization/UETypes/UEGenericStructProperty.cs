using GvasFormat.Serializer;
using GvasFormat.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("Count = {Properties.Count}", Name = "{Name}")]
    public sealed class UEGenericStructProperty : UEStructProperty
    {
        public List<UEProperty> Properties = new List<UEProperty>();

        public UEGenericStructProperty() { }

        public UEGenericStructProperty(GvasReader reader, string name, string type, string structType, long valueLength) : base(name, type, structType, valueLength)
        {
            while (UESerializer.Deserialize(reader) is UEProperty prop)
            {
                Properties.Add(prop);
                if (prop is UENoneProperty)
                    break;
            }
        }

        public override long SerializeStructProp(GvasWriter writer)
        {
            long size = 0;
            foreach (UEProperty prop in Properties)
            {
                size += prop.Serialize(writer);
            }
            return size;
        }
    }
}
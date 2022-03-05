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

        public UEGenericStructProperty(BinaryReader reader)
        {
            while (Deserialize(reader) is UEProperty prop)
            {
                Properties.Add(prop);
                if (prop is UENoneProperty)
                    break;
            }
        }

        public override void SerializeStructProp(BinaryWriter writer)
        {
            foreach (UEProperty prop in Properties)
            {
                prop.Serialize(writer);
            }
        }
    }
}
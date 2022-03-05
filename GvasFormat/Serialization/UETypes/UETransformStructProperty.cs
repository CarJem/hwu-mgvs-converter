using System;
using System.Collections.Generic;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    internal class UETransformStructProperty : UEStructProperty
    {
        public List<UEProperty> Transform = new List<UEProperty>();
        public List<UEProperty> Model = new List<UEProperty>();

        public UETransformStructProperty() { }

        public UETransformStructProperty(BinaryReader reader)
        {
            while (Deserialize(reader) is UEProperty prop)
            {
                Transform.Add(prop);
                if (prop is UENoneProperty)
                    break;
            }
            while (Deserialize(reader) is UEProperty prop)
            {
                Model.Add(prop);
                if (prop is UENoneProperty)
                    break;
            }
        }

        public override void SerializeStructProp(BinaryWriter writer)
        {
            foreach (UEProperty prop in Transform)
            {
                prop.Serialize(writer);
            }
            foreach (UEProperty prop in Model)
            {
                prop.Serialize(writer);
            }
        }
    }
}
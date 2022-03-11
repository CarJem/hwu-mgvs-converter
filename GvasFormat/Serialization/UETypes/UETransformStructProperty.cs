using GvasFormat.Serializer;
using GvasFormat.Utils;
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

        public UETransformStructProperty(GvasReader reader, string name, string type, string structType, long valueLength) : base(name, type, structType, valueLength)
        {
            while (UESerializer.Deserialize(reader) is UEProperty prop)
            {
                Transform.Add(prop);
                if (prop is UENoneProperty)
                    break;
            }
            while (UESerializer.Deserialize(reader) is UEProperty prop)
            {
                Model.Add(prop);
                if (prop is UENoneProperty)
                    break;
            }
        }

        public override long SerializeStructProp(GvasWriter writer)
        {
            long size = 0;
            foreach (UEProperty prop in Transform)
            {
                size += prop.Serialize(writer);
            }
            foreach (UEProperty prop in Model)
            {
                size += prop.Serialize(writer);
            }
            return size;
        }
    }
}
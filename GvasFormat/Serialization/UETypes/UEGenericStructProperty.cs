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

        public UEGenericStructProperty(BinaryReader reader, string name, string type, string structType, long valueLength) : base(name, type, structType, valueLength)
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

        public static UEProperty[] DeserializeArray(BinaryReader reader, string name, string type, string arrayType, long valueLength)
        {
            List<UEProperty> array = new List<UEProperty>();
            var terminator = reader.ReadTerminator();
            var arrayLength = reader.ReadInt32();
            for (int i = 0; i < arrayLength; i++)
            {
                var item = UESerializer.DeserializeProperty(null, arrayType, -1, reader);
                array.Add(item);
            }
            return array.ToArray();
        }

        public static void SerializeArray(BinaryWriter writer, UEProperty[] Items)
        {
            writer.Write(false); //terminator
            writer.WriteInt32(Items.Length);
            for (int i = 0; i < Items.Length; i++)
            {
                UEProperty prop = Items[i];
                if (i == 0) { prop.Serialize(writer); }
                else { prop.SerializeProp(writer); }
            }
        }
    }
}
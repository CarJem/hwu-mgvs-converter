using GvasFormat.Serialization.HotWheels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("Count = {Items.Length}", Name = "{Name}")]
    public sealed class UEArrayProperty : UEProperty
    {
        public UEArrayProperty() { }
        public UEArrayProperty(BinaryReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            ItemType = reader.ReadUEString();
            DeserializeArray(reader, name, type, valueLength);
        }

        private void DeserializeArray(BinaryReader reader, string name, string type, long valueLength)
        {
            var array = new List<UEProperty>();

            if (UESerializer.CanDeserializeHWUSpecialByteArray(ItemType, name))
                array.Add(UESerializer.DeserializeHWUSpecialByteArray(reader, ItemType, name));

            else if (ItemType == "ByteProperty")
                array.Add(UESerializer.DeserializeProperty(null, ItemType, valueLength, reader));

            else
            {
                var terminator = reader.ReadTerminator();
                var arrayLength = reader.ReadInt32();

                for (int i = 0; i < arrayLength; i++)
                {
                    if (i == 0 && ItemType != "NameProperty") 
                        array.Add(Deserialize(reader));

                    else if (ItemType == "StructProperty") 
                        array.Add(UESerializer.DeserializeStruct(null, ((UEStructProperty)array[0]).Type, ((UEStructProperty)array[0]).StructType, -1, reader));

                    else 
                        array.Add(UESerializer.DeserializeProperty(name, ItemType, -1, reader));

                    if (UEHomelessString.Exists(reader)) 
                        array.Add(new UEHomelessString());
                }
            }

            Items = array.ToArray();
        }

        public override void SerializeProp(BinaryWriter writer)
        { 
            writer.WriteUEString(ItemType);

            if (UESerializer.CanSerializeHWUSpecialByteArray(ItemType, Items, Name))
                UESerializer.SerializeHWUSpecialByteArray(writer, Items, ItemType, Name);

            else if (ItemType == "ByteProperty")
                Items.ToList().ForEach(x => x.Serialize(writer));

            else
            {
                writer.Write(false); //terminator
                writer.WriteInt32(Items.Length);

                for (int i = 0; i < Items.Length; i++)
                {
                    UEProperty prop = Items[i];

                    if (i == 0 && ItemType != "NameProperty")
                        prop.Serialize(writer);

                    else if (prop is UEStructProperty)
                        ((UEStructProperty)prop).SerializeStructProp(writer);

                    else
                        prop.SerializeProp(writer);
                }
            }

        }

        public string ItemType;
        public UEProperty[] Items;
    }
}
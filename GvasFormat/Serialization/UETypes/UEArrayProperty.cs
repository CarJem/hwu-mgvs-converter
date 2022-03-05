using System;
using System.Diagnostics;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("Count = {Items.Length}", Name = "{Name}")]
    public sealed class UEArrayProperty : UEProperty
    {
        public UEArrayProperty() { }
        public UEArrayProperty(BinaryReader reader, long valueLength)
        {
            ItemType = reader.ReadUEString();
            Items = UESerializer.DeserializeArray(ItemType, valueLength, reader);
        }

        public override void SerializeProp(BinaryWriter writer)
        { 
            writer.WriteUEString(ItemType);
            UESerializer.SerializeArray(writer, Items, ItemType);
        }

        public string ItemType;
        public UEProperty[] Items;
    }
}
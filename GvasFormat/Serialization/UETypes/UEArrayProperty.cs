using System;
using System.Diagnostics;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("Count = {Items.Length}", Name = "{Name}")]
    public sealed class UEArrayProperty : UEProperty
    {
        public UEArrayProperty() { }
        public UEArrayProperty(BinaryReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            ItemType = reader.ReadUEString();
            Items = UESerializer.DeserializeArray(ItemType, name, type, valueLength, reader);
        }
        public override void SerializeMap(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void SerializeProp(BinaryWriter writer)
        { 
            writer.WriteUEString(ItemType);
            UESerializer.SerializeArray(writer, Items, Name, ItemType);
        }

        public string ItemType;
        public UEProperty[] Items;
    }
}
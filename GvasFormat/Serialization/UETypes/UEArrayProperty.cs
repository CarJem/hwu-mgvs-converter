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
            var terminator = reader.ReadByte();
            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            // valueLength starts here
            Count = reader.ReadInt32();
            Items = new UEProperty[Count];

            switch (ItemType)
            {
                case "StructProperty":
                    Items = Read(reader, Count);
                    break;
                case "ByteProperty":
                    Items = UEByteProperty.Read(reader, valueLength, Count);
                    break;
                default:
                    {
                        for (var i = 0; i < Count; i++)
                            Items[i] = UESerializer.Deserialize(null, ItemType, -1, reader);
                        break;
                    }
            }
        }

        public override void SerializeProp(BinaryWriter writer)
        {
            writer.WriteUEString(ItemType);
            writer.Write(false); //terminator
            writer.WriteInt32(Count);

            for (int i=0; i<Items.Length; i++)
            {
                UEProperty prop = Items[i];
                if (i == 0) { prop.Serialize(writer); }
                else if (prop is UEStructProperty) { ((UEStructProperty)prop).SerializeStructProp(writer); }
                else { prop.SerializeProp(writer); }
            }
        }

        public string ItemType;
        public UEProperty[] Items;
        public int Count;
    }
}
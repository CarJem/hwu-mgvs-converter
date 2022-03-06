using System;
using System.Collections.Generic;
using System.IO;
using GvasFormat.Serialization.HWTypes;
using GvasFormat.Serialization.UETypes;

namespace GvasFormat.Serialization
{
    public static partial class UESerializer
    {
        internal static UEProperty[] DeserializeArray(string arrayType, long valueLength, BinaryReader reader)
        {
            List<UEProperty> array;

            switch (arrayType)
            {
                case "ByteProperty":
                    array = DeseralizeByteProperty(arrayType, valueLength, reader);
                    break;
                case "StructProperty":
                    array = DeseralizeStructProperty(arrayType, valueLength, reader);
                    break;
                case "NameProperty":
                    array = DeseralizeNameProperty(arrayType, valueLength, reader);
                    break;
                default:
                    array = DeseralizeGenericProperty(arrayType, valueLength, reader);
                    break;
            }

      
            return array.ToArray();
        }

        private static List<UEProperty> DeseralizeGenericProperty(string arrayType, long valueLength, BinaryReader reader)
        {
            List<UEProperty> array = new List<UEProperty>();
            var terminator = reader.ReadTerminator();
            var arrayLength = reader.ReadInt32();
            for (int i = 0; i < arrayLength; i++)
            {
                var item = DeserializeProperty(null, arrayType, -1, reader);
                array.Add(item);
            }
            return array;
        }
        private static List<UEProperty> DeseralizeNameProperty(string arrayType, long valueLength, BinaryReader reader)
        {
            List<UEProperty> array = new List<UEProperty>();
            var terminator = reader.ReadTerminator();
            var arrayLength = reader.ReadInt32();
            for (int i = 0; i < arrayLength; i++)
            {
                var item = DeserializeProperty(null, arrayType, -1, reader);
                array.Add(item);
            }
            return array;
        }
        private static List<UEProperty> DeseralizeByteProperty(string arrayType, long valueLength, BinaryReader reader)
        {
            List<UEProperty> array = new List<UEProperty>();
            array.Add(DeserializeProperty(null, arrayType, valueLength, reader));
            return array;
        }
        private static List<UEProperty> DeseralizeStructProperty(string arrayType, long valueLength, BinaryReader reader)
        {
            List<UEProperty> array = new List<UEProperty>();

            var terminator = reader.ReadTerminator();
            var arrayLength = reader.ReadInt32();
            for (int i = 0; i < arrayLength; i++)
            {
                if (i == 0) array.Add(UEProperty.Deserialize(reader));
                else
                {
                    var structType = (array[0] as UEStructProperty).StructType;
                    var item = DeserializeStruct(structType, arrayLength, reader);
                    item.Name = array[0].Name;
                    item.Type = array[0].Type;
                    array.Add(item);

                    BlankTest();
                }
            }

            return array;

            void BlankTest()
            {
                var initalPosition = reader.BaseStream.Position;

                var oneInterger = reader.ReadInt32();
                var noneString = reader.ReadUEString(5, true);

                if (oneInterger != 1 || noneString != "None")
                    reader.BaseStream.Position = initalPosition;
            }
        }


        internal static void SerializeArray(BinaryWriter writer, UEProperty[] Items, string ItemType)
        {
            switch (ItemType)
            {
                case "StructProperty":
                    SeralizeStructProperty(writer, Items, ItemType);
                    break;
                case "ByteProperty":
                    SeralizeByteProperty(writer, Items, ItemType);
                    break;
                case "NameProperty":
                    SeralizeNameProperty(writer, Items, ItemType);
                    break;
                default:
                    SeralizeGenericProperty(writer, Items, ItemType);
                    break;
            }
        }

        private static void SeralizeGenericProperty(BinaryWriter writer, UEProperty[] Items, string ItemType)
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
        private static void SeralizeNameProperty(BinaryWriter writer, UEProperty[] Items, string ItemType)
        {
            writer.Write(false); //terminator
            writer.WriteInt32(Items.Length);
            for (int i = 0; i < Items.Length; i++)
            {
                UENameProperty prop = Items[i] as UENameProperty;
                prop.SerializeProp(writer, true);
            }
        }
        private static void SeralizeByteProperty(BinaryWriter writer, UEProperty[] Items, string ItemType)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                UEProperty prop = Items[i];
                if (i == 0) { prop.Serialize(writer); }
                else { prop.SerializeProp(writer); }
            }
        }
        private static void SeralizeStructProperty(BinaryWriter writer, UEProperty[] Items, string ItemType)
        {
            writer.Write(false); //terminator
            writer.WriteInt32(Items.Length);
            for (int i = 0; i < Items.Length; i++)
            {
                UEProperty prop = Items[i];
                if (i == 0) { prop.Serialize(writer); }
                else if (prop is UEStructProperty) { ((UEStructProperty)prop).SerializeStructProp(writer); }
                else { prop.SerializeProp(writer); }
            }
        }
    }
}
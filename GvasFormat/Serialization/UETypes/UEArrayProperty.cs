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
        public string ItemType;
        public Dictionary<string, UEArrayHeader> Headers = new Dictionary<string, UEArrayHeader>();
        public UEProperty[] Items;

        public UEArrayProperty() { }

        public UEArrayProperty(GvasReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            ItemType = reader.ReadUEString();
            var terminator = reader.ReadTerminator();
            Headers = DeserializeHeaders(reader, ItemType);

            int Size;
            if (ItemType == "ByteProperty") Size = 1;
            else Size = (int)Headers["Size"].GetValue();

            var array = new List<UEProperty>();
            for (int i = 0; i < Size; i++)
            {
                if (ItemType == "StructProperty")
                {
                    var headerType = (string)Headers["Type"].GetValue();
                    var headerStruct = (string)Headers["StructureType"].GetValue();
                    array.Add(UESerializer.DeserializeStruct(null, headerType, headerStruct, -1, reader));
                }
                else if (UESerializer.CanDeserializeHWUSpecialByteArray(ItemType, name))
                {
                    array.Add(UESerializer.DeserializeHWUSpecialByteArray(reader, ItemType, name));
                }
                else
                {
                    array.Add(UESerializer.DeserializeProperty(null, ItemType, -1, reader));
                } 

                if (UEHomelessString.Exists(reader))
                    array.Add(new UEHomelessString());
            }

            Headers = Headers.Where(x => x.Value.JsonIgnore == false).ToDictionary(x => x.Key, x => x.Value);
            Items = array.ToArray();
        }
        private Dictionary<string, UEArrayHeader> DeserializeHeaders(GvasReader reader, string itemType)
        {
            var headers = new Dictionary<string, UEArrayProperty.UEArrayHeader>();
            switch (itemType)
            {
                case "StructProperty":
                    headers.Add("Size", new UEArrayProperty.UEArrayHeader(reader.ReadInt32(), true));
                    headers.Add("Name", new UEArrayProperty.UEArrayHeader(reader.ReadUEString()));
                    headers.Add("Type", new UEArrayProperty.UEArrayHeader(reader.ReadUEString()));
                    headers.Add("ValueLength", new UEArrayProperty.UEArrayHeader(reader.ReadInt64(), true));
                    headers.Add("StructureType", new UEArrayProperty.UEArrayHeader(reader.ReadUEString()));
                    headers.Add("UUID", new UEArrayProperty.UEArrayHeader(reader.ReadBytes(16)));
                    headers.Add("Terminator", new UEArrayProperty.UEArrayHeader(reader.ReadTerminator()));
                    break;
                case "ByteProperty":
                    break;
                default:
                    headers.Add("Size", new UEArrayProperty.UEArrayHeader(reader.ReadInt32()));
                    break;
            }
            return headers;
        }

        public override long SerializeProp(GvasWriter writer)
        {
            long size = 0;
            long headerSize = 0;
            long arraySize = 0;

            writer.WriteUEString(ItemType);
            writer.Write(false); //terminator

            var memory = new MemoryStream();
            using (var data = new GvasWriter(memory, System.Text.Encoding.Default, true))
            {
                for (int i = 0; i < Items.Length; i++)
                {
                    var item = Items[i];

                    if (item is UEStructProperty)
                        arraySize += (item as UEStructProperty).SerializeStructProp(data);

                    else if (UESerializer.CanSerializeHWUSpecialByteArray(ItemType, Items, Name))
                        arraySize += UESerializer.SerializeHWUSpecialByteArray(data, item, ItemType, Name);

                    else arraySize += item.SerializeProp(data);
                }
            }
            headerSize += SerializeHeaders(writer, ItemType, arraySize);
            size += writer.Write(memory.ToArray());
            memory.Close();

            return size + headerSize;
        }
        private long SerializeHeaders(GvasWriter writer, string itemType, long valueLength)
        {
            long size = 0;
            switch (itemType)
            {
                case "StructProperty":

                    //NOTE: Size Should be calculated : size += writer.WriteInt32((int)headers["Size"].GetValue());
                    size += writer.WriteInt32(Items.Length);
                    size += writer.WriteUEString((string)Headers["Name"].GetValue());
                    size += writer.WriteUEString((string)Headers["Type"].GetValue());
                    //NOTE: ValueLength should be calculated : size += writer.WriteInt64((long)headers["ValueLength"].GetValue());
                    size += writer.WriteInt64(valueLength);
                    size += writer.WriteUEString((string)Headers["StructureType"].GetValue());
                    size += writer.Write((byte[])Headers["UUID"].GetValue());
                    size += writer.Write((byte)Headers["Terminator"].GetValue());
                    break;
                case "ByteProperty":
                    break;
                default:
                    //NOTE: Size Should be calculated : size += writer.WriteInt32((int)headers["Size"].GetValue());
                    size += writer.WriteInt32(Items.Length);
                    break;
            }
            return size;
        }

        public class UEArrayHeader
        {
            public object Value;
            public string ValueType;
            public bool JsonIgnore;
            public UEArrayHeader()
            {
                Value = null;
                ValueType = "null";
                JsonIgnore = false;
            }
            public UEArrayHeader(byte[] value, bool jsonIgnore = false)
            {
                Value = HexExtensions.ToHexString(value);
                ValueType = "byte[]";
                JsonIgnore = jsonIgnore;
            }
            public UEArrayHeader(byte value, bool jsonIgnore = false)
            {
                Value = value;
                ValueType = "byte";
                JsonIgnore = jsonIgnore;
            }
            public UEArrayHeader(int value, bool jsonIgnore = false)
            {
                Value = value;
                ValueType = "int";
                JsonIgnore = jsonIgnore;
            }
            public UEArrayHeader(long value, bool jsonIgnore = false)
            {
                Value = value;
                ValueType = "long";
                JsonIgnore = jsonIgnore;
            }
            public UEArrayHeader(string value, bool jsonIgnore = false)
            {
                Value = value;
                ValueType = "string";
                JsonIgnore = jsonIgnore;
            }

            public object GetValue()
            {
                switch (ValueType)
                {
                    case "string":
                        return Value.ToString();
                    case "long":
                        return long.Parse(Value.ToString());
                    case "int":
                        return int.Parse(Value.ToString());
                    case "byte":
                        return byte.Parse(Value.ToString());
                    case "byte[]":
                        return HexExtensions.FromHexString(Value.ToString());
                    default:
                        return Value;
                }
            }
        }
    }
}
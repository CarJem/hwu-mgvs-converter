using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GvasFormat.Serialization.HotWheels;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("Count = {Map.Count}", Name = "{Name}")]
    public sealed class UEMapProperty : UEProperty
    {
        public UEMapProperty() { }
        public UEMapProperty(GvasReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            KeyType = reader.ReadUEString();
            ValueType = reader.ReadUEString();

            var terminator = reader.ReadTerminator();
            var unknown = reader.ReadBytes(4);
            if (unknown.Any(b => b != 0))
                throw new InvalidOperationException($"Offset: 0x{reader.BaseStream.Position-4:x8}. Expected ??? to be 0, but was 0x{unknown}");

            var count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                UEProperty key, value;
                if (KeyType == "StructProperty") key = UESerializer.Deserialize(reader);
                else key = UESerializer.DeserializeProperty(null, KeyType, -1, reader);

                var values = new List<UEProperty>();
                if (ValueType == "StructProperty")
                {
                    do
                    {
                        if (UESerializer.DoesSpecialHWUStructExists(reader))
                            value = UESerializer.DeserializeSpecialHWUStruct(reader, null, ValueType, TileMarketingDownloadedTexture.PropertyName, -1);

                        else value = UESerializer.Deserialize(reader);

                        values.Add(value);
                    }
                    while (!(value is UENoneProperty) && !UESerializer.IsHWUMappedStructTerminator(value));
                }
                else
                {
                    value = UESerializer.DeserializeProperty(null, ValueType, -1, reader);
                    values.Add(value);
                }

                    Map.Add(new UEKeyValuePair{Key = key, Values = values});
            }
            if (count == 0)
            {
                UESerializer.Deserialize(reader);
            }
        }

        public override long SerializeProp(GvasWriter writer)
        {
            long size = 0;
            writer.WriteUEString(KeyType);
            writer.WriteUEString(ValueType);
            writer.Write(false); //terminator
            size += writer.WriteInt32(0); //unknown
            size += writer.WriteInt32(Map.Count);

            foreach (UEKeyValuePair p in Map)
            {
                if (KeyType == "StructProperty") size += p.Key.Serialize(writer);
                else size += p.Key.SerializeProp(writer);

                foreach (UEProperty v in p.Values)
                {
                    if (ValueType == "StructProperty") size += v.Serialize(writer);
                    else size += v.SerializeProp(writer);
                }
            }
            if (Map.Count == 0)
            {
                size += new UENoneProperty().SerializeProp(writer);
            }
            return size;
        }

        public List<UEKeyValuePair> Map = new List<UEKeyValuePair>();
        public string KeyType;
        public string ValueType;

        public class UEKeyValuePair
        {
            public UEProperty Key;
            public List<UEProperty> Values;
        }
    }
}
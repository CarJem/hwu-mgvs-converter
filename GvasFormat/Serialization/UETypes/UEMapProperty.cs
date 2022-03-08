using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GvasFormat.Utils;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("Count = {Map.Count}", Name = "{Name}")]
    public sealed class UEMapProperty : UEProperty
    {
        public UEMapProperty() { }
        public UEMapProperty(BinaryReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            KeyType = reader.ReadUEString();
            ValueType = reader.ReadUEString();
            var unknown = reader.ReadBytes(5);
            if (unknown.Any(b => b != 0))
                throw new InvalidOperationException($"Offset: 0x{reader.BaseStream.Position-5:x8}. Expected ??? to be 0, but was 0x{unknown}");

            var count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                UEProperty key, value;
                if (KeyType == "StructProperty")
                    key = Deserialize(reader);
                else
                    key = UESerializer.DeserializeProperty(null, KeyType, -1, reader);

                var values = new List<UEProperty>();
                if (ValueType == "StructProperty")
                    value = Deserialize(reader);
                else
                    value = UESerializer.DeserializeProperty(null, ValueType, -1, reader);
                values.Add(value);
                //do {} while (!(value is UENoneProperty));
                Map.Add(new UEKeyValuePair{Key = key, Values = values});
            }
            if (count == 0)
            {
                //Read(reader);
                Deserialize(reader);
            }
        }
        public override void SerializeMap(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void SerializeProp(BinaryWriter writer)
        {
            writer.WriteUEString(KeyType);
            writer.WriteUEString(ValueType);
            writer.WriteInt32(0); //unknown
            writer.Write(false); //unknown
            writer.WriteInt32(Map.Count);

            foreach (UEKeyValuePair p in Map)
            {
                p.Key.SerializeMap(writer);

                foreach (UEProperty v in p.Values)
                {
                    v.SerializeMap(writer);
                }
            }
            if (Map.Count == 0)
            {
               //new UENoneProperty().Serialize(writer);
               new UENoneProperty().Serialize(writer);
            }
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
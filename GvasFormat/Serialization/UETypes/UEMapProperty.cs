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
        public UEMapProperty(BinaryReader reader, long valueLength)
        {
            KeyType = reader.ReadUEString();
            ValueType = reader.ReadUEString();
            var unknown = reader.ReadBytes(5);
            if (unknown.Any(b => b != 0))
                throw new InvalidOperationException($"Offset: 0x{reader.BaseStream.Position-5:x8}. Expected ??? to be 0, but was 0x{unknown.AsHex()}");

            var count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                UEProperty key, value;
                if (KeyType == "StructProperty")
                    key = Read(reader);
                else
                    key = UESerializer.Deserialize(null, KeyType, -1, reader);
                var values = new List<UEProperty>();
                do
                {
                    if (ValueType == "StructProperty")
                        value = Read(reader);
                    else
                        value = UESerializer.Deserialize(null, ValueType, -1, reader);
                    values.Add(value);
                } while (!(value is UENoneProperty));
                Map.Add(new UEKeyValuePair{Key = key, Values = values});
            }
            if (count == 0)
            {
                //Read(reader);
                Read(reader);
            }
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
                p.Key.Serialize(writer);

                foreach (UEProperty v in p.Values)
                {
                    v.Serialize(writer);
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
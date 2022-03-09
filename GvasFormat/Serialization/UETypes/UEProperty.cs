using GvasFormat.Serialization.HotWheels;
using System;
using System.Collections.Generic;
using System.IO;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    public abstract class UEProperty
    {
        public string Name;
        public string Type;
        public long ValueLength;

        public UEProperty() { }
        public UEProperty(string name, string type, long valueLength)
        {
            Name = name;
            Type = type;
            ValueLength = valueLength;
        }

        public abstract void SerializeProp(BinaryWriter writer);

        internal static UEProperty Deserialize(BinaryReader reader)
        {
            var peeked = reader.PeekChar();
            if (peeked < 0)
                return null;

            var name = reader.ReadUEString();

            if (name == null)
                return null;

            if (name == UENoneProperty.PropertyName)
                return new UENoneProperty { Name = name };

            var type = reader.ReadUEString();

            if (name == string.Empty && type == UENoneProperty.PropertyName)
                return new UEHomelessString();

            var valueLength = reader.ReadInt64();
            return UESerializer.DeserializeProperty(name, type, valueLength, reader);
        }
        public void Serialize(BinaryWriter writer)
        {
            if (Name == UENoneProperty.PropertyName || Name == UEHomelessString.PropertyName || Name == null || UESerializer.IsHWUSpecialSerializable(Name, Type))
            {
                SerializeProp(writer);
            }
            else
            {
                writer.WriteUEString(Name);
                writer.WriteUEString(Type);
                writer.WriteInt64(ValueLength);
                SerializeProp(writer);
            }
        }
    }
}
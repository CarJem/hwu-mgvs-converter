using System;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    public abstract class UEProperty
    {
        public string Name;
        public string Type;
        public long ValueLength;

        public abstract void SerializeProp(BinaryWriter writer);

        public void Serialize(BinaryWriter writer)
        {
            if (Name == "None" || Name == null)
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

        public static UEProperty Read(BinaryReader reader)
        {
            if (reader.PeekChar() < 0)
                return null;

            var name = reader.ReadUEString();
            if (name == null)
                return null;

            if (name == "None")
                return new UENoneProperty { Name = name };

            var type = reader.ReadUEString();
            var valueLength = reader.ReadInt64();
            return UESerializer.Deserialize(name, type, valueLength, reader);
        }

        public static UEProperty[] Read(BinaryReader reader, int count)
        {
            if (reader.PeekChar() < 0)
                return null;

            var name = reader.ReadUEString();
            var type = reader.ReadUEString();
            var valueLength = reader.ReadInt64();
            return UESerializer.Deserialize(name, type, valueLength, count, reader);
        }
    }
}
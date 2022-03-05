using System;
using System.Collections.Generic;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    public abstract class UEProperty
    {
        public string Name;
        public string Type;
        public long ValueLength;

        public abstract void SerializeProp(BinaryWriter writer);

        internal static UEProperty Deserialize(BinaryReader reader)
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
            return UESerializer.DeserializeProperty(name, type, valueLength, reader);
        }
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
    }
}
using GvasFormat.Serialization.HotWheels;
using System;
using System.Collections.Generic;
using System.IO;
using GvasFormat.Utils;
using GvasFormat.Serializer;
using Newtonsoft.Json;

namespace GvasFormat.Serialization.UETypes
{
    public abstract class UEProperty
    {
        public string Name;
        public string Type;
        public bool Indexed = false;

        [JsonIgnore] 
        private long ValueLength;

        public UEProperty() { }
        public UEProperty(string name, string type, long valueLength)
        {
            Name = name;
            Type = type;
            ValueLength = valueLength;
            if (valueLength == -1) Indexed = true;
        }

        public abstract long SerializeProp(GvasWriter writer);
        public long Serialize(GvasWriter writer, bool bodyLengthOnly = false)
        {

            if (Name == UENoneProperty.PropertyName || Name == null || UESerializer.IsHWUSpecialSerializable(Name, Type))
            {
                return SerializeProp(writer);
            }
            else
            {
                long size = 0;

                size += writer.WriteUEString(Name);
                size += writer.WriteUEString(Type);

                var memory = new MemoryStream();
                long length = 0;
                using (var data = new GvasWriter(memory, System.Text.Encoding.Default, true)) length = SerializeProp(data);
                size += writer.WriteInt64(length);
                size += writer.Write(memory.ToArray());
                memory.Close();

                return size;
            }
        }
    }
}
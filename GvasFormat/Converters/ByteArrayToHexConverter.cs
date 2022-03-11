using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace GvasFormat.Converters
{
    public class ByteArrayToHexConverter : JsonConverter
    {
        public ByteArrayToHexConverter()
        {
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(byte[]));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string hexString = reader.Value.ToString();
            return Utils.HexExtensions.FromHexString(hexString);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string hex = Utils.HexExtensions.ToHexString((byte[])value);
            writer.WriteValue(hex);
        }
    }
}
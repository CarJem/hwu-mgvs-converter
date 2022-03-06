using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace GvasConverter
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
            byte[] retval = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i += 2)
                retval[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            return retval;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string hex = BitConverter.ToString((byte[])value).Replace("-", string.Empty);
            writer.WriteValue(hex);
        }
    }
}
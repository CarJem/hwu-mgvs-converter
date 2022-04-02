using GvasFormat;
using GvasFormat.Serialization.HotWheels;
using GvasFormat.Serialization.UETypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace GvasFormat.Converters
{
    public class GvasJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(UEProperty));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            string type = jo[nameof(UEProperty.Type)].Value<string>();
            string name = jo[nameof(UEProperty.Name)].Value<string>();

            switch (type)
            {
                case "DoubleProperty":
                    return jo.ToObject<UEDoubleProperty>(serializer);
                case "Int64Property":
                    return jo.ToObject<UEInt64Property>(serializer);
                case "BoolProperty":
                    return jo.ToObject<UEBoolProperty>(serializer);
                case "IntProperty":
                    return jo.ToObject<UEIntProperty>(serializer);
                case "FloatProperty":
                    return jo.ToObject<UEFloatProperty>(serializer);
                case "NameProperty":
                    return jo.ToObject<UENameProperty>(serializer);
                case "StrProperty":
                    return jo.ToObject<UEStringProperty>(serializer);
                case "SoftObjectProperty":
                    return jo.ToObject<UEObjectProperty>(serializer);
                case "ObjectProperty":
                    return jo.ToObject<UEObjectProperty>(serializer);
                case "TextProperty":
                    return jo.ToObject<UETextProperty>(serializer);
                case "EnumProperty":
                    return jo.ToObject<UEEnumProperty>(serializer);
                case "StructProperty":
                    return ReadStructJson();
                case "ArrayProperty":
                    return jo.ToObject<UEArrayProperty>(serializer);
                case "MapProperty":
                    return jo.ToObject<UEMapProperty>(serializer);
                case "SetProperty":
                    return jo.ToObject<UESetProperty>(serializer);
                case "ByteProperty":
                    return jo.ToObject<UEByteProperty>(serializer);
                case DownloadedLiveries.PropertyType:
                    return jo.ToObject<DownloadedLiveries>(serializer);
                default:
                    if (name == UENoneProperty.PropertyName) return jo.ToObject<UENoneProperty>(serializer);
                    else if (name == UEHomelessString.PropertyName) return jo.ToObject<UEHomelessString>(serializer);
                    else throw new FormatException($"Unknown value type '{type}'");
            }

            object ReadStructJson()
            {
                string structType = jo[nameof(UEStructProperty.StructType)].Value<string>();
                switch (structType)
                {
                    case "DateTime":
                        return jo.ToObject<UEDateTimeStructProperty>(serializer);
                    case "Guid":
                        return jo.ToObject<UEGuidStructProperty>(serializer);
                    case "Vector":
                        return jo.ToObject<UEVectorStructProperty>(serializer);
                    case "Rotator":
                        return jo.ToObject<UEVectorStructProperty>(serializer);
                    case "LinearColor":
                        return jo.ToObject<UELinearColorStructProperty>(serializer);
                    case "Transform":
                        return jo.ToObject<UETransformStructProperty>(serializer);
                    case "Quat":
                        return jo.ToObject<UEQuaternionStructProperty>(serializer);
                    case "VehicleEditorProject":
                        return jo.ToObject<VehicleEditorProject>(serializer);
                    case TileMarketingDownloadedTexture.PropertyName:
                        return jo.ToObject<TileMarketingDownloadedTexture>(serializer);
                    default:
                        return jo.ToObject<UEGenericStructProperty>(serializer);
                }
            }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

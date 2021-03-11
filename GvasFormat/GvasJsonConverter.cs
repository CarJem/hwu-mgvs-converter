using GvasFormat;
using GvasFormat.Serialization.UETypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace GvasConverter
{
    public class GvasJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Gvas).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Gvas gvas = existingValue == null ? new Gvas() : (Gvas)existingValue;

            JObject jo = JObject.Load(reader);
            gvas.SaveGameVersion = int.Parse(jo["SaveGameVersion"].ToString());
            gvas.PackageVersion = int.Parse(jo["PackageVersion"].ToString());
            gvas.EngineVersion.Major = short.Parse(jo["EngineVersion"]["Major"].ToString());
            gvas.EngineVersion.Minor = short.Parse(jo["EngineVersion"]["Minor"].ToString());
            gvas.EngineVersion.Patch = short.Parse(jo["EngineVersion"]["Patch"].ToString());
            gvas.EngineVersion.Build = short.Parse(jo["EngineVersion"]["Build"].ToString());
            gvas.EngineVersion.BuildId = jo["EngineVersion"]["BuildId"].ToString();
            gvas.CustomFormatVersion = int.Parse(jo["CustomFormatVersion"].ToString());
            gvas.CustomFormatData.Count = int.Parse(jo["CustomFormatData"]["Count"].ToString());

            List<CustomFormatDataEntry> cfd = new List<CustomFormatDataEntry>();
            foreach (JObject o in jo["CustomFormatData"]["Entries"])
            {
                cfd.Add(new CustomFormatDataEntry(o["Id"].ToObject<Guid>(), int.Parse(o["Value"].ToString())));
            }
            gvas.CustomFormatData.Entries = cfd.ToArray();
            gvas.SaveGameType = jo["SaveGameType"].ToString();

            foreach (JObject o in jo["Properties"])
            {
                gvas.Properties.Add(ReadUEProperty(o));
            }
            //gvas.Properties.Add(ReadUEProperty(o));

            return gvas;
        }

        public UEProperty ReadUEProperty(JObject o)
        {
            string t = o["Type"].ToString();
            string n = o["Name"].ToString();
            if (n == "")
                n = null;

            if (n == "None")
            {
                return new UENoneProperty();
            }

            UEProperty value;

            switch (t)
            {
                case "BoolProperty":
                    value = ReadUEBoolProperty(o);
                    break;
                case "IntProperty":
                    value = ReadUEIntProperty(o);
                    break;
                case "FloatProperty":
                    value = ReadUEFloatProperty(o);
                    break;
                case "NameProperty":
                case "StrProperty":
                case "SoftObjectProperty":
                case "ObjectProperty":
                    value = ReadUEStringProperty(o);
                    break;
                case "TextProperty":
                    value = ReadUETextProperty(o);
                    break;
                case "EnumProperty":
                    value = ReadUEEnumProperty(o);
                    break;
                case "StructProperty":
                    value = ReadUEStructProperty(o);
                    break;
                case "ArrayProperty":
                    value = ReadUEArrayProperty(o);
                    break;
                case "MapProperty":
                    value = ReadUEMapProperty(o);
                    break;
                case "ByteProperty":
                    value = ReadUEByteProperty(o);
                    break;
                default:
                    throw new FormatException($"Property {n} has unknown type {t}");
            }
            value.Type = t;
            value.Name = n;
            value.ValueLength = long.Parse(o["ValueLength"].ToString());

            return value;

        }

        private UEFloatProperty ReadUEFloatProperty(JObject o)
        {
            UEFloatProperty value = new UEFloatProperty();

            value.Value = float.Parse(o["Value"].ToString());

            return value;
        }

        private UEIntProperty ReadUEIntProperty(JObject o)
        {
            UEIntProperty value = new UEIntProperty();

            value.Value = int.Parse(o["Value"].ToString());

            return value;
        }

        private UEBoolProperty ReadUEBoolProperty(JObject o)
        {
            var value = new UEBoolProperty();
            value.Value = bool.Parse(o["Value"].ToString());
            return value;
        }

        private UEEnumProperty ReadUEEnumProperty(JObject o)
        {
            throw new NotImplementedException();
        }

        private UEByteProperty ReadUEByteProperty(JObject o)
        {
            throw new NotImplementedException();
        }

        private UEMapProperty ReadUEMapProperty(JObject o)
        {
            UEMapProperty value = new UEMapProperty();

            value.KeyType = o["KeyType"].ToString();
            value.ValueType = o["ValueType"].ToString();

            foreach (JObject p in o["Map"].Children())
            {
                UEMapProperty.UEKeyValuePair pair = new UEMapProperty.UEKeyValuePair();
                pair.Key = ReadUEProperty((JObject)p["Key"]);
                pair.Values = new List<UEProperty>();

                foreach (JObject v in p["Values"])
                {
                    pair.Values.Add(ReadUEProperty(v));
                }

                value.Map.Add(pair);
            }

            return value;
        }

        private UETextProperty ReadUETextProperty(JObject o)
        {
            UETextProperty value = new UETextProperty();
            value.Flags = long.Parse(o["Flags"].ToString());
            value.Id = o["Id"].ToString();
            value.Value = o["Value"].ToString();
            return value;
        }

        private UEStringProperty ReadUEStringProperty(JObject o)
        {
            UEStringProperty value = new UEStringProperty();
            value.Value = o["Value"].ToString();
            return value;
        }

        public UEArrayProperty ReadUEArrayProperty(JObject o)
        {
            UEArrayProperty value = new UEArrayProperty();
            value.ItemType = o["ItemType"].ToString();
            value.Count = int.Parse(o["Count"].ToString());
            List<UEProperty> items = new List<UEProperty>();
            foreach (JObject item in o["Items"])
            {
                items.Add(ReadUEProperty(item));
            }
            value.Items = items.ToArray();
            return value;
        }

        private UEStructProperty ReadUEStructProperty(JObject o)
        {
            UEStructProperty value;

            string st = o["StructType"].ToString();

            switch(st)
            {
                case "DateTime":
                    value = ReadUEDateTimeStructProperty(o);
                    break;
                case "Guid":
                    value = ReadUEGuidStructProperty(o);
                    break;
                case "Vector":
                case "Rotator":
                    value = ReadUEVectorStructProperty(o);
                    break;
                case "LinearColor":
                    value = ReadUELinearColorStructProperty(o);
                    break;
                /*case "Transform":
                    value = ReadUETransformStructProperty(o);
                    break;*/
                case "Quat":
                    value = ReadUEQuaternionStructProperty(o);
                    break;
                default:
                    value = ReadUEGenericStructProperty(o);
                    break;
            }
            value.StructType = st;


            return value;
        }

        private UEStructProperty ReadUETransformStructProperty(JObject o)
        {
            UETransformStructProperty value = new UETransformStructProperty();

            foreach (JObject prop in o["Transform"])
            {
                value.Transform.Add(ReadUEProperty(prop));
            }
            foreach (JObject prop in o["Model"])
            {
                value.Model.Add(ReadUEProperty(prop));
            }
            return value;
        }

        private UEGuidStructProperty ReadUEGuidStructProperty(JObject o)
        {
            throw new NotImplementedException();
        }

        private UEVectorStructProperty ReadUEVectorStructProperty(JObject o)
        {
            UEVectorStructProperty value = new UEVectorStructProperty();

            value.X = float.Parse(o["X"].ToString());
            value.Y = float.Parse(o["Y"].ToString());
            value.Z = float.Parse(o["Z"].ToString());

            return value;
        }

        private UEQuaternionStructProperty ReadUEQuaternionStructProperty(JObject o)
        {
            UEQuaternionStructProperty value = new UEQuaternionStructProperty();

            value.X = float.Parse(o["X"].ToString());
            value.Y = float.Parse(o["Y"].ToString());
            value.I = float.Parse(o["I"].ToString());
            value.J = float.Parse(o["J"].ToString());

            return value;
        }

        private UELinearColorStructProperty ReadUELinearColorStructProperty(JObject o)
        {
            UELinearColorStructProperty value = new UELinearColorStructProperty();
            if (int.Parse(o["ValueLength"].ToString()) == 0) return value;

            value.R = float.Parse(o["R"].ToString());
            value.G = float.Parse(o["G"].ToString());
            value.B = float.Parse(o["B"].ToString());
            value.A = float.Parse(o["A"].ToString());

            return value;
        }

        private UEDateTimeStructProperty ReadUEDateTimeStructProperty(JObject o)
        {
            UEDateTimeStructProperty value = new UEDateTimeStructProperty();
            value.Value = o["Value"].ToObject<DateTime>();
            return value;
        }

        private UEGenericStructProperty ReadUEGenericStructProperty(JObject o)
        {
            UEGenericStructProperty value = new UEGenericStructProperty();

            foreach (JObject prop in o["Properties"])
            {
                value.Properties.Add(ReadUEProperty(prop));
            }

            return value;

        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

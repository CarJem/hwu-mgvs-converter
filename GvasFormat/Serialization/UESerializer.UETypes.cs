using System;
using System.Collections.Generic;
using System.IO;
using GvasFormat.Serialization.HWTypes;
using GvasFormat.Serialization.UETypes;

namespace GvasFormat.Serialization
{
    public static partial class UESerializer
    {
        internal static UEProperty DeserializeProperty(string name, string type, long valueLength, BinaryReader reader)
        {
            UEProperty result;
            var itemOffset = reader.BaseStream.Position;
            switch (type)
            {
                case "BoolProperty":
                    result = new UEBoolProperty(reader, valueLength);
                    break;
                case "IntProperty":
                    result = new UEIntProperty(reader, valueLength);
                    break;
                case "FloatProperty":
                    result = new UEFloatProperty(reader, valueLength);
                    break;
                case "NameProperty":
                    result = new UENameProperty(reader, valueLength);
                    break;
                case "StrProperty":
                    result = new UEStringProperty(reader, valueLength);
                    break;
                case "SoftObjectProperty":
                    result = new UEObjectProperty(reader, valueLength);
                    break;
                case "ObjectProperty":
                    result = new UEObjectProperty(reader, valueLength);
                    break;
                case "TextProperty":
                    result = new UETextProperty(reader, valueLength);
                    break;
                case "EnumProperty":
                    result = new UEEnumProperty(reader, valueLength);
                    break;
                case "StructProperty":
                    result = UEStructProperty.Read(reader, valueLength);
                    break;
                case "ArrayProperty":
                    result = new UEArrayProperty(reader, valueLength);
                    break;
                case "MapProperty":
                    result = new UEMapProperty(reader, valueLength);
                    break;
                case "SetProperty":
                    result = new UESetProperty(reader, valueLength);
                    break;
                case "ByteProperty":
                    result = UEByteProperty.Read(reader, valueLength);
                    break;
                default:
                    throw new FormatException($"Offset: 0x{itemOffset:x8}. Unknown value type '{type}' of item '{name}'");
            }
            result.Name = name;
            result.Type = type;
            result.ValueLength = valueLength;
            return result;
        }
        internal static UEStructProperty DeserializeStruct(string type, long valueLength, BinaryReader reader)
        {
            UEStructProperty result;
            switch (type)
            {
                case "DateTime":
                    result = new UEDateTimeStructProperty(reader);
                    break;
                case "Guid":
                    result = new UEGuidStructProperty(reader);
                    break;
                case "Vector":
                    result = new UEVectorStructProperty(reader);
                    break;
                case "Rotator":
                    result = new UEVectorStructProperty(reader);
                    break;
                case "LinearColor":
                    result = new UELinearColorStructProperty(reader);
                    break;
                case "Transform":
                    result = new UETransformStructProperty(reader);
                    break;
                case "Quat":
                    result = new UEQuaternionStructProperty(reader);
                    break;
                case "VehicleEditorProject":
                    result = new HWUVehicleEditorProject(reader);
                    break;
                default:
                    result = new UEGenericStructProperty(reader);
                    break;
            }
            result.StructType = type;
            result.ValueLength = valueLength;
            return result;
        }
    }
}
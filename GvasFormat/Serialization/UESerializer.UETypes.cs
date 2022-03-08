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
                    result = new UEBoolProperty(reader, name, type, valueLength);
                    break;
                case "IntProperty":
                    result = new UEIntProperty(reader, name, type, valueLength);
                    break;
                case "Int64Property":
                    result = new UEInt64Property(reader, name, type, valueLength);
                    break;
                case "DoubleProperty":
                    result = new UEDoubleProperty(reader, name, type, valueLength);
                    break;
                case "FloatProperty":
                    result = new UEFloatProperty(reader, name, type, valueLength);
                    break;
                case "NameProperty":
                    result = new UENameProperty(reader, name, type, valueLength);
                    break;
                case "StrProperty":
                    result = new UEStringProperty(reader, name, type, valueLength);
                    break;
                case "SoftObjectProperty":
                    result = new UEObjectProperty(reader, name, type, valueLength);
                    break;
                case "ObjectProperty":
                    result = new UEObjectProperty(reader, name, type, valueLength);
                    break;
                case "TextProperty":
                    result = new UETextProperty(reader, name, type, valueLength);
                    break;
                case "EnumProperty":
                    result = new UEEnumProperty(reader, name, type, valueLength);
                    break;
                case "ArrayProperty":
                    result = new UEArrayProperty(reader, name, type, valueLength);
                    break;
                case "MapProperty":
                    result = new UEMapProperty(reader, name, type, valueLength);
                    break;
                case "SetProperty":
                    result = new UESetProperty(reader, name, type, valueLength);
                    break;
                case "ByteProperty":
                    result = new UEByteProperty(reader, name, type, valueLength);
                    break;
                case "StructProperty":
                    result = UEStructProperty.Read(reader, name, type, valueLength);
                    break;
                default:
                    throw new FormatException($"Offset: 0x{itemOffset:x8}. Unknown value type '{type}' of item '{name}'");
            }
            return result;
        }
        internal static UEStructProperty DeserializeStruct(string name, string type, string structType, long valueLength, BinaryReader reader)
        {
            UEStructProperty result;
            switch (structType)
            {
                case "DateTime":
                    result = new UEDateTimeStructProperty(reader, name, type, structType, valueLength);
                    break;
                case "Guid":
                    result = new UEGuidStructProperty(reader, name, type, structType, valueLength);
                    break;
                case "Vector":
                    result = new UEVectorStructProperty(reader, name, type, structType, valueLength);
                    break;
                case "Rotator":
                    result = new UEVectorStructProperty(reader, name, type, structType, valueLength);
                    break;
                case "LinearColor":
                    result = new UELinearColorStructProperty(reader, name, type, structType, valueLength);
                    break;
                case "Transform":
                    result = new UETransformStructProperty(reader, name, type, structType, valueLength);
                    break;
                case "Quat":
                    result = new UEQuaternionStructProperty(reader, name, type, structType, valueLength);
                    break;
                case "VehicleEditorProject":
                    result = new HWUVehicleEditorProject(reader, name, type, structType, valueLength);
                    break;
                default:
                    result = new UEGenericStructProperty(reader, name, type, structType, valueLength);
                    break;
            }
            return result;
        }
        internal static UEProperty[] DeserializeArray(string arrayType, string name, string type, long valueLength, BinaryReader reader)
        {
            if (HWUDownloadedLiveries.CanDeserialize(arrayType, name))
                return new UEProperty[] { new HWUDownloadedLiveries(reader, name) };

            UEProperty[] array;

            switch (arrayType)
            {
                case "ByteProperty":
                    array = UEByteProperty.DeserializeArray(reader, name, type, arrayType, valueLength);
                    break;
                case "StructProperty":
                    array = UEStructProperty.DeserializeArray(reader, name, type, arrayType, valueLength);
                    break;
                case "NameProperty":
                    array = UENameProperty.DeserializeArray(reader, name, type, arrayType, valueLength);
                    break;
                default:
                    array = UEGenericStructProperty.DeserializeArray(reader, name, type, arrayType, valueLength);
                    break;
            }

            return array;
        }

        internal static void SerializeArray(BinaryWriter writer, UEProperty[] items, string name, string arrayType)
        {
            if (HWUDownloadedLiveries.CanSerialize(arrayType, items, name))
            {
                items[0].SerializeProp(writer);
                return;
            }

            switch (arrayType)
            {
                case "StructProperty":
                    UEStructProperty.SerializeArray(writer, items);
                    break;
                case "ByteProperty":
                    UEByteProperty.SerializeArray(writer, items);
                    break;
                case "NameProperty":
                    UENameProperty.SerializeArray(writer, items);
                    break;
                default:
                    UEGenericStructProperty.SerializeArray(writer, items);
                    break;
            }
        }
    }
}
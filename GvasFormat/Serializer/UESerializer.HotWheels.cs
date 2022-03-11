using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GvasFormat.Serialization.HotWheels;
using GvasFormat.Serialization.UETypes;
using GvasFormat.Serialization;
using GvasFormat.Utils;

namespace GvasFormat.Serializer
{
    public static partial class UESerializer
    {
        internal static UEProperty DeserializeHWUSpecialByteArray(GvasReader reader, string itemType, string name)
        {
            return new HWUDownloadedLiveries(reader, name);
        }
        /// <param name="valueLength">Leave as -1 for properties that are nested within another</param>
        internal static UEProperty DeserializeSpecialHWUStruct(GvasReader reader, string name, string valueType, string propertyName, int valueLength)
        {
            return new TileMarketingDownloadedTexture(reader, null, valueType, TileMarketingDownloadedTexture.PropertyName, -1);      
        }
        /// <param name="valueLength">Leave as -1 for properties that are nested within another</param>
        internal static UEStructProperty DeserializeHWUSpecialStruct(GvasReader reader, string name, string type, string structType, long valueLength)
        {
            UEStructProperty result;
            switch (structType)
            {
                case "VehicleEditorProject":
                    result = new HWUVehicleEditorProject(reader, name, type, structType, valueLength);
                    break;
                default:
                    result = null;
                    break;
            }
            return result;
        }

        internal static bool CanDeserializeHWUSpecialByteArray(string itemType, string name)
        {
            return HWUDownloadedLiveries.CanDeserialize(itemType, name);
        }
        internal static bool CanSerializeHWUSpecialByteArray(string itemType, UEProperty[] items, string name)
        {
            return HWUDownloadedLiveries.CanSerialize(itemType, items, name);
        }
        internal static bool IsHWUSpecialSerializable(string name, string type)
        {
            return name == TileMarketingDownloadedTexture.PropertyName ||
                type == HWUDownloadedLiveries.PropertyType;
        }
        internal static bool IsHWUMappedStructTerminator(UEProperty value)
        {
            return (value is TileMarketingDownloadedTexture);
        }
        internal static bool IsHWUDirectSeralizableStructureProperty(string structType)
        {
            return structType.Equals(TileMarketingDownloadedTexture.PropertyName);
        }
        internal static bool DoesSpecialHWUStructExists(GvasReader reader)
        {
            return TileMarketingDownloadedTexture.Exists(reader);
        }

        internal static long SerializeHWUSpecialByteArray(GvasWriter writer, UEProperty item, string itemType, string name)
        {
            return item.SerializeProp(writer);
        }
    }
}

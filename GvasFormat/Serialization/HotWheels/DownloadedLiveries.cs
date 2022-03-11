using System;
using System.Collections.Generic;
using System.IO;
using GvasFormat.Serialization.UETypes;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.HotWheels
{
    public class HWUDownloadedLiveries : UEProperty
    {
        public const string PropertyType = "HWUDownloadedLiveries";
        public const string OriginalType = "ByteProperty";
        public const string OriginalName = "DownloadedLiveries";
        public List<HWULiveryRemoteGameData> Items { get; set; } = new List<HWULiveryRemoteGameData>();

        public HWUDownloadedLiveries()
        {
            Type = PropertyType;
            Name = OriginalName;
        }   

        public HWUDownloadedLiveries(GvasReader reader, string name) : base (name, PropertyType, -1)
        {
            var arrayLength = reader.ReadInt32();
            var size = reader.ReadInt32();

            for (int i = 0; i < size; i++)
            {
                Items.Add(new HWULiveryRemoteGameData(reader));
            }
        }


        public override long SerializeProp(GvasWriter writer)
        {
            long size = 0;

            var memory = new MemoryStream();
            long byteLength = 0;
            using (var data = new GvasWriter(memory, System.Text.Encoding.Default, true))
            {
                byteLength += data.WriteInt32(Items.Count);
                for (int i = 0; i < Items.Count; i++)
                    byteLength += Items[i].SerializeProp(data);
            }

            size += writer.WriteInt32((int)byteLength);
            writer.Write(memory.ToArray());
            memory.Close();

            return byteLength + size;
        }
        public static bool CanDeserialize(string arrayType, string name)
        {
            return arrayType == OriginalType && name == OriginalName;
        }

        public static bool CanSerialize(string arrayType, UEProperty[] items, string name)
        {
            return arrayType == PropertyType && name == OriginalName && items.Length == 1;
        }
    }

}
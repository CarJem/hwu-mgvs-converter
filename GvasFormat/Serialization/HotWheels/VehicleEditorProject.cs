using GvasFormat.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GvasFormat.Serialization.UETypes;
using GvasFormat.Serializer;
using Newtonsoft.Json;

namespace GvasFormat.Serialization.HotWheels
{
    [DebuggerDisplay("{Vehicle}", Name = "{LiveryName}")]
    public class HWUVehicleEditorProject : UEStructProperty
    {
        public const bool DEV_DoNotModifyDataChunks = true;
        public HWUVehicleEditorProject() { }
        public HWUVehicleEditorProject(GvasReader reader, string name, string type, string structType, long valueLength) : base(name, type, structType, valueLength)
        {
            Guid = new Guid(reader.ReadBytes(16));
            LiveryName = reader.ReadUEString();

            var TagCount = reader.ReadInt32();

            for (int i = 0; i < TagCount; i++)
                Tags.Add(reader.ReadUEString());

            Unknown = reader.ReadBytes(14);

            var ChunkCount = reader.ReadInt16();
            isChunkSizeOdd = ChunkCount % 2 == 1;

            MaxDecompressedDataSize = reader.ReadInt64();
            MaxCompressedDataSize = reader.ReadInt64();
            var TotalCompressedDataSize = reader.ReadInt64();
            var TotalDecompressedDataSize = reader.ReadInt64();

            var DataCount = DeserializeDataChunkCount(ChunkCount);
            var SizeCalculations = new List<KeyValuePair<long, long>>();

            for (int i = 0; i < DataCount; i++)
                SizeCalculations.Add(new KeyValuePair<long, long>(reader.ReadInt64(), reader.ReadInt64()));

            for (int i = 0; i < DataCount; i++)
                DataChunks.Add(new DataChunk(reader, SizeCalculations[i].Key, SizeCalculations[i].Value));

            Material = reader.ReadUEString();

            Something1 = reader.ReadInt32();
            Something2 = reader.ReadInt32();
            Something3 = reader.ReadInt32();
            Something4 = reader.ReadInt32();
            Something5 = reader.ReadByte();

            var JFIFSize = reader.ReadInt64();
            JFIFData = reader.ReadBytes((int)JFIFSize);

            UEProperty prop;
            while ((prop = UESerializer.Deserialize(reader)).Name != UENoneProperty.PropertyName)
                Properties.Add(prop);

            Vehicle = reader.ReadUEString();

            if (Tags.Contains("Versioned"))
            {
                var versionCount = reader.ReadInt32();

                for (int i = 0; i < versionCount; i++)
                    Versions.Add(reader.ReadUEString());
            }
        }

        public override long SerializeStructProp(GvasWriter writer)
        {
            long size = 0;
            size += writer.Write(Guid.ToByteArray());
            size += writer.WriteUEString(LiveryName);
            size += writer.WriteInt32(Tags.Count);

            for (int i = 0; i < Tags.Count; i++)
                size += writer.WriteUEString(Tags[i]);

            size += writer.Write(Unknown);

            size += writer.WriteInt16(SerializeDataChunkCount());
            size += writer.WriteInt64(MaxDecompressedDataSize);
            size += writer.WriteInt64(MaxCompressedDataSize);

            size += writer.WriteInt64(GetTotalCompressedDataSize());
            size += writer.WriteInt64(GetTotalDecompressedDataSize());

            for (int i = 0; i < DataChunks.Count; i++)
            {
                size += writer.WriteInt64(DataChunks[i].CompressedSize);
                size += writer.WriteInt64(DataChunks[i].DecompressedSize);
            }

            for (int i = 0; i < DataChunks.Count; i++)
                size += writer.Write(DataChunks[i].CompressedData);

            size += writer.WriteUEString(Material);

            size += writer.WriteInt32(Something1);
            size += writer.WriteInt32(Something2);
            size += writer.WriteInt32(Something3);
            size += writer.WriteInt32(Something4);
            size += writer.Write(Something5);

            size += writer.WriteInt64(JFIFData.Length);
            size += writer.Write(JFIFData);

            for (int i = 0; i < Properties.Count; i++)
                size += Properties[i].Serialize(writer);
            size += writer.WriteUENoneProperty();

            size += writer.WriteUEString(Vehicle);

            if (Tags.Contains("Versioned"))
            {
                size += writer.WriteInt32(Versions.Count);
                for (int i = 0; i < Versions.Count; i++)
                    size += writer.WriteUEString(Versions[i]);
            }

            return size;
        }

        #region Helpers
        private short DeserializeDataChunkCount(short ChunkCount)
        {
            var setCounts = ChunkCount - (ChunkCount % 2); //subtract the remainder for an unknown reason
            setCounts += 2; //add 2 for the always existant data set's values
            setCounts /= 2; //divide by 2 to get the number of sets
            return (short)(setCounts);
        }
        private short SerializeDataChunkCount()
        {
            var setCounts = DataChunks.Count - 1; //subtract one for the always existant data set
            setCounts *= 2; //multiply by two to get the number of values
            setCounts += isChunkSizeOdd ? 1 : 0; //add the remainder for an unknown reason
            return (short)(setCounts);
        }
        public long GetTotalCompressedDataSize()
        {
            long result = 0;
            if (DataChunks != null) DataChunks.ForEach(x => result += x.CompressedSize);
            return result;
        }
        public long GetTotalDecompressedDataSize()
        {
            long result = 0;
            if (DataChunks != null) DataChunks.ForEach(x => result += x.DecompressedSize);
            return result;
        }
        #endregion

        public Guid Guid { get; set; }
        public string LiveryName { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public byte[] Unknown { get; set; }
        public bool isChunkSizeOdd { get; set; }
        public Int64 MaxDecompressedDataSize { get; set; }
        public Int64 MaxCompressedDataSize { get; set; }
        public List<DataChunk> DataChunks { get; set; } = new List<DataChunk>();
        public string Material { get; set; }
        public int Something1 { get; set; }
        public int Something2 { get; set; }
        public int Something3 { get; set; }
        public int Something4 { get; set; }
        public byte Something5 { get; set; }
        public byte[] JFIFData { get; set; }
        public List<UEProperty> Properties { get; set; } = new List<UEProperty>();
        public string Vehicle { get; set; }
        public List<string> Versions { get; set; } = new List<string>();

        public class DataChunk
        {
            public DataChunk() { }
            public DataChunk(GvasReader reader, long compressedSize, long initalDecompressedSize)
            {
                UnmodifiedCompressedData = reader.ReadBytes((int)compressedSize);
                DecompressedData = ZLibExtensions.Decompress(UnmodifiedCompressedData);
                if (DecompressedData.Length != initalDecompressedSize)
                    throw new FormatException($"Mismatching Decompressed Length! Expected {initalDecompressedSize} but got {DecompressedData.Length}");
            }
            public byte[] UnmodifiedCompressedData { get; set; }
            public byte[] DecompressedData { get; set; }
            public byte[] CompressedData
            {
                get
                {
                    #pragma warning disable CS0162 // Unreachable code detected
                    if (DEV_DoNotModifyDataChunks) return UnmodifiedCompressedData;
                    else return ZLibExtensions.Compress(DecompressedData);
                    #pragma warning restore CS0162 // Unreachable code detected
                }
            }
       
            public long CompressedSize => CompressedData.Length;
            public long DecompressedSize => DecompressedData.Length;
            
        }
    }

}
using GvasFormat.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GvasFormat.Serialization.UETypes;
using GvasFormat.Serializer;
using Newtonsoft.Json;
using GvasFormat.Settings;

namespace GvasFormat.Serialization.HotWheels
{
    [DebuggerDisplay("{Vehicle}", Name = "{LiveryName}")]
    public class VehicleEditorProject : UEStructProperty
    {
        public VehicleEditorProject() { }
        public VehicleEditorProject(GvasReader reader, string name, string type, string structType, long valueLength) : base(name, type, structType, valueLength)
        {
            Guid = new Guid(reader.ReadBytes(16));
            LiveryName = reader.ReadUEString();

            var TagCount = reader.ReadInt32();

            for (int i = 0; i < TagCount; i++)
                Tags.Add(reader.ReadUEString());

            UnknownA = reader.ReadBytes(14);

            Projection = ProjectionData.Read(LiveryName, reader);

            Material = reader.ReadUEString();

            UnknownB = reader.ReadInt32();
            UnknownC = reader.ReadInt32();
            UnknownD = reader.ReadInt32();
            UnknownE = reader.ReadInt32();
            UnknownF = reader.ReadByte();

            var JFIFSize = reader.ReadInt64();
            DisplayImage = reader.ReadBytes((int)JFIFSize);

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

            size += writer.Write(UnknownA);

            size += Projection.Write(writer);

            size += writer.WriteUEString(Material);

            size += writer.WriteInt32(UnknownB);
            size += writer.WriteInt32(UnknownC);
            size += writer.WriteInt32(UnknownD);
            size += writer.WriteInt32(UnknownE);
            size += writer.Write(UnknownF);

            size += writer.WriteInt64(DisplayImage.Length);
            size += writer.Write(DisplayImage);

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

        public Guid Guid { get; set; }
        public string LiveryName { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public byte[] UnknownA { get; set; }
        public ProjectionData Projection { get; set; }
        public string Material { get; set; }
        public int UnknownB { get; set; }
        public int UnknownC { get; set; }
        public int UnknownD { get; set; }
        public int UnknownE { get; set; }
        public byte UnknownF { get; set; }
        public byte[] DisplayImage { get; set; }
        public List<UEProperty> Properties { get; set; } = new List<UEProperty>();
        public string Vehicle { get; set; }
        public List<string> Versions { get; set; } = new List<string>();


        public class ProjectionData
        {
            public long MaxCompressedLength { get; set; }
            public long MaxDecompressedLength { get; set; }

            public List<ProjectionPureChunks> DataChunks { get; set; } = new List<ProjectionPureChunks>();

            public List<LayerCoordinate> Coordinates { get; set; } = new List<LayerCoordinate>();
            public List<LayerProperties> Properties { get; set; } = new List<LayerProperties>();
            public List<LayerEmptySpace> EmptySpaces { get; set; } = new List<LayerEmptySpace>();
            public List<LayerCameraShot> CameraShots { get; set; } = new List<LayerCameraShot>();


            public ProjectionData() : base() { }

            public static ProjectionData Read(string LiveryName, GvasReader reader)
            {
                ProjectionData result = new ProjectionData();
                var ChunkCount = reader.ReadInt16();

                result.MaxCompressedLength = reader.ReadInt64();
                result.MaxDecompressedLength = reader.ReadInt64();
                var TotalCompressedDataSize = reader.ReadInt64();
                var TotalDecompressedDataSize = reader.ReadInt64();

                var DataCount = DeserializeDataChunkCount(ChunkCount);
                var SizeCalculations = new List<KeyValuePair<long, long>>();

                for (int i = 0; i < DataCount; i++)
                    SizeCalculations.Add(new KeyValuePair<long, long>(reader.ReadInt64(), reader.ReadInt64()));

                for (int i = 0; i < DataCount; i++)
                    result.DataChunks.Add(new ProjectionPureChunks(reader, SizeCalculations[i].Key, SizeCalculations[i].Value));

                if (GvasSettings.LiveryProjectionPraseOption == GvasSettings.LiveryProjectionPraseMode.Layers)
                    Read_SeperateLayers(ref result, LiveryName, reader);


                return result;

                short DeserializeDataChunkCount(short count)
                {
                    var setCounts = count - (count % 2); //subtract the remainder for an unknown reason
                    setCounts += 2; //add 2 for the always existant data set's values
                    setCounts /= 2; //divide by 2 to get the number of sets
                    return (short)(setCounts);
                }
            }
            public static void Read_SeperateLayers(ref ProjectionData result, string LiveryName, GvasReader reader)
            {
                List<byte> data = new List<byte>();
                result.DataChunks.ForEach(x => data.AddRange(x.DecompressedData));
                result.DataChunks.Clear();
                Stream stream = new MemoryStream(data.ToArray());

                using (var decompressedReader = new GvasReader(stream))
                {
                    var Terminator = decompressedReader.ReadByte();
                    var LayerCountA = decompressedReader.ReadUInt16();

                    for (int i = 0; i < LayerCountA; i++)
                        result.Coordinates.Add(LayerCoordinate.Read(decompressedReader));

                    for (int i = 0; i < LayerCountA; i++)
                        result.Properties.Add(LayerProperties.Read(decompressedReader));

                    var LayerCountC = decompressedReader.ReadUInt16();

                    for (int i = 0; i < LayerCountC; i++)
                        result.EmptySpaces.Add(LayerEmptySpace.Read(decompressedReader));

                    var LayerCountD = decompressedReader.ReadUInt16();

                    for (int i = 0; i < LayerCountD; i++)
                        result.CameraShots.Add(LayerCameraShot.Read(decompressedReader));


                    if (decompressedReader.BaseStream.Position != decompressedReader.BaseStream.Length)
                        throw new Exception($"Did not reach end of stream for the \"{LiveryName}\" livery!\r\n" +
                            $"Should of been {decompressedReader.BaseStream.Length} got {decompressedReader.BaseStream.Position}!");
                }
            }

            public long Write(GvasWriter writer)
            {
                if (GvasSettings.LiveryProjectionPraseOption == GvasSettings.LiveryProjectionPraseMode.Layers)
                {
                    string CacheFile = Path.GetTempFileName();
                    Write_CombineLayers(CacheFile, out long PreChunkedDataSize);
                    Write_ChunkLayerData(CacheFile, PreChunkedDataSize);
                    File.Delete(CacheFile);
                }


                long size = 0;
                size += writer.WriteInt16(SerializeDataChunkCount());
                size += writer.WriteInt64(MaxCompressedLength);
                size += writer.WriteInt64(MaxDecompressedLength);

                size += writer.WriteInt64(GetTotalCompressedDataSize());
                size += writer.WriteInt64(GetTotalDecompressedDataSize());

                for (int i = 0; i < DataChunks.Count; i++)
                {
                    size += writer.WriteInt64(DataChunks[i].CompressedSize);
                    size += writer.WriteInt64(DataChunks[i].DecompressedSize);
                }

                for (int i = 0; i < DataChunks.Count; i++)
                    size += writer.Write(DataChunks[i].CompressedData);

                return size;

                short SerializeDataChunkCount()
                {
                    var setCounts = DataChunks.Count - 1; //subtract one for the always existant data set
                    setCounts *= 2; //multiply by two to get the number of values
                    setCounts += (DataChunks.Count % 2 == 1) ? 1 : 0; //add the remainder for an unknown reason
                    return (short)(setCounts);
                }
                long GetTotalCompressedDataSize()
                {
                    long result = 0;
                    if (DataChunks != null) DataChunks.ForEach(x => result += x.CompressedSize);
                    return result;
                }
                long GetTotalDecompressedDataSize()
                {
                    long result = 0;
                    if (DataChunks != null) DataChunks.ForEach(x => result += x.DecompressedSize);
                    return result;
                }

            }
            public void Write_CombineLayers(string CacheFile, out long PreChunkedDataSize)
            {
                long length;

                using (Stream stream = new FileStream(CacheFile, FileMode.Create))
                {
                    using (GvasWriter subWriter = new GvasWriter(stream))
                    {
                        subWriter.Write(false); //terminator
                        subWriter.WriteUInt16((ushort)Coordinates.Count);

                        for (int i = 0; i < Coordinates.Count; i++)
                            Coordinates[i].Write(subWriter);

                        for (int i = 0; i < Coordinates.Count; i++)
                            Properties[i].Write(subWriter);

                        subWriter.WriteUInt16((ushort)EmptySpaces.Count);

                        for (int i = 0; i < EmptySpaces.Count; i++)
                            EmptySpaces[i].Write(subWriter);

                        subWriter.WriteUInt16((ushort)CameraShots.Count);

                        for (int i = 0; i < CameraShots.Count; i++)
                            CameraShots[i].Write(subWriter);

                        length = stream.Length;
                    }
                }

                PreChunkedDataSize = length;
            }
            public void Write_ChunkLayerData(string CacheFile, long PreChunkedDataSize)
            {
                int count = 0;
                long leftoverSize = 0;

                while (PreChunkedDataSize != 0)
                {
                    if (PreChunkedDataSize >= MaxDecompressedLength)
                    {
                        PreChunkedDataSize -= MaxDecompressedLength;
                        count++;
                    }
                    else
                    {
                        leftoverSize = PreChunkedDataSize;
                        PreChunkedDataSize -= PreChunkedDataSize;
                    }
                }

                using (Stream stream = new FileStream(CacheFile, FileMode.Open))
                {
                    using (GvasReader subReader = new GvasReader(stream))
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var chunkData = subReader.ReadBytes((int)MaxDecompressedLength);
                            DataChunks.Add(new ProjectionPureChunks() { DecompressedData = chunkData });
                        }

                        if (leftoverSize > 0)
                        {
                            var chunkData = subReader.ReadBytes((int)leftoverSize);
                            DataChunks.Add(new ProjectionPureChunks() { DecompressedData = chunkData });
                        }
                    }
                }
            }

            #region Classes

            public class ProjectionPureChunks
            {
                public ProjectionPureChunks() { }
                public ProjectionPureChunks(GvasReader reader, long compressedSize, long initalDecompressedSize)
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
                        if (GvasSettings.LiveryProjectionPraseOption == GvasSettings.LiveryProjectionPraseMode.Layers) 
                            return ZLibExtensions.Compress(DecompressedData);
                        else
                            return UnmodifiedCompressedData;
                    }
                }

                public long CompressedSize => CompressedData.Length;
                public long DecompressedSize => DecompressedData.Length;

            }
            public class LayerCoordinate
            {
                public byte[] X;
                public byte[] Y;
                public byte[] Depth;
                public byte[] ScaleX;
                public byte[] ScaleY;
                public byte[] Rotation;
                public LayerCoordinate() { }
                public static LayerCoordinate Read(GvasReader reader)
                {
                    LayerCoordinate result = new LayerCoordinate();
                    result.X = reader.ReadBytes(2);
                    result.Y = reader.ReadBytes(2);
                    result.Depth = reader.ReadBytes(2);
                    result.ScaleX = reader.ReadBytes(2);
                    result.ScaleY = reader.ReadBytes(2);
                    result.Rotation = reader.ReadBytes(2);
                    return result;
                }

                public long Write(GvasWriter writer)
                {
                    long length = 0;

                    length += writer.Write(X);
                    length += writer.Write(Y);
                    length += writer.Write(Depth);
                    length += writer.Write(ScaleX);
                    length += writer.Write(ScaleY);
                    length += writer.Write(Rotation);

                    return length;
                }
            }
            public class LayerProperties
            {
                public byte[] ShotIndex;
                public byte[] LiveryShape;
                public byte[] ColorR;
                public byte[] ColorG;
                public byte[] ColorB;
                public byte[] UnknownA;
                public byte[] Opacity;
                public byte[] InclinationX;
                public byte[] InclinationY;
                public byte Orientation;
                public byte Limits;
                public byte[] UnknownB;

                public LayerProperties() { }
                public static LayerProperties Read(GvasReader reader)
                {
                    LayerProperties result = new LayerProperties();
                    result.ShotIndex = reader.ReadBytes(2);
                    result.LiveryShape = reader.ReadBytes(2);
                    result.ColorR = reader.ReadBytes(4);
                    result.ColorG = reader.ReadBytes(4);
                    result.ColorB = reader.ReadBytes(4);
                    result.UnknownA = reader.ReadBytes(4);
                    result.Opacity = reader.ReadBytes(2);
                    result.InclinationX = reader.ReadBytes(2);
                    result.InclinationY = reader.ReadBytes(2);
                    result.Orientation = reader.ReadByte();
                    result.Limits = reader.ReadByte();
                    result.UnknownB = reader.ReadBytes(2);
                    return result;
                }
                public long Write(GvasWriter writer)
                {
                    long length = 0;

                    length += writer.Write(ShotIndex);
                    length += writer.Write(LiveryShape);
                    length += writer.Write(ColorR);
                    length += writer.Write(ColorG);
                    length += writer.Write(ColorB);
                    length += writer.Write(UnknownA);
                    length += writer.Write(Opacity);
                    length += writer.Write(InclinationX);
                    length += writer.Write(InclinationY);
                    length += writer.Write(Orientation);
                    length += writer.Write(Limits);
                    length += writer.Write(UnknownB);

                    return length;
                }
            }
            public class LayerEmptySpace
            {
                public byte[] Data;
                public LayerEmptySpace() { }
                public static LayerEmptySpace Read(GvasReader reader)
                {
                    LayerEmptySpace result = new LayerEmptySpace();
                    result.Data = reader.ReadBytes(5);
                    return result;
                }
                public long Write(GvasWriter writer)
                {
                    long length = 0;

                    length += writer.Write(Data);

                    return length;
                }
            }
            public class LayerCameraShot
            {
                public byte[] Unknown;

                public LayerCameraShot() { }
                public static LayerCameraShot Read(GvasReader reader)
                {
                    LayerCameraShot result = new LayerCameraShot();
                    result.Unknown = reader.ReadBytes(14);
                    return result;
                }
                public long Write(GvasWriter writer)
                {
                    long length = 0;

                    length += writer.Write(Unknown);

                    return length;
                }
            }

            #endregion
        }

    }

}
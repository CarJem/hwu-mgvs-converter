using GvasFormat.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GvasFormat.Serialization.UETypes;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.HotWheels
{
    [DebuggerDisplay("{Vehicle}", Name = "{LiveryName}")]
    public class HWUVehicleEditorProject : UEStructProperty
    {

        public Guid Guid { get; set; }
        public string LiveryName { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public Int64 UnknownUUID { get; set; }
        public byte[] Unknown_00 { get; set; }
        public byte Unknown_04 { get; set; }
        public byte Unknown_05 { get; set; }
        public byte SomeIterable { get; set; }
        public byte Unknown_07 { get; set; }
        public byte[] Unknown_08 { get; set; }
        public byte[] Unknown_10 { get; set; }
        public Int64 DataSize { get; set; }
        public Int64 Unknown_18 { get; set; }
        public byte[] Unknown_20 { get; set; } = new byte[0];
        public Int64 Unknown_LongA { get; set; }
        public Int64 Unknown_LongB { get; set; }
        public byte[] Data { get; set; }
        public string Material { get; set; }
        public byte[] JFIFJunk { get; set; }
        public byte[] JFIFData { get; set; }
        public List<UEProperty> Properties { get; set; } = new List<UEProperty>();
        public string Vehicle { get; set; }
        public List<string> Versions { get; set; } = new List<string>();

        public HWUVehicleEditorProject() { }
        public HWUVehicleEditorProject(GvasReader reader, string name, string type, string structType, long valueLength) : base(name, type, structType, valueLength)
        {
            Guid = new Guid(reader.ReadBytes(16));
            LiveryName = reader.ReadUEString();

            var TagCount = reader.ReadInt32();

            for (int i = 0; i < TagCount; i++)
                Tags.Add(reader.ReadUEString());

            UnknownUUID = reader.ReadInt64();
            Unknown_00 = reader.ReadBytes(4);
            Unknown_04 = reader.ReadByte();
            Unknown_05 = reader.ReadByte();
            SomeIterable = reader.ReadByte();
            Unknown_07 = reader.ReadByte();
            Unknown_08 = reader.ReadBytes(8);
            Unknown_10 = reader.ReadBytes(8);

            DataSize = reader.ReadInt64();
            Unknown_18 = reader.ReadInt64();

            int arraySize = SomeIterable - (int)SomeIterable % 2;
            if (SomeIterable != 0) Unknown_20 = reader.ReadBytes(8 * arraySize);

            Unknown_LongA = reader.ReadInt64();
            Unknown_LongB = reader.ReadInt64();

            Data = reader.ReadBytes((int)DataSize);
            Material = reader.ReadUEString();
            
            JFIFJunk = reader.ReadBytes(25);
            JFIFData = reader.ReadJFIF();

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

            size += writer.WriteInt64(UnknownUUID);
            size += writer.Write(Unknown_00);
            size += writer.Write(Unknown_04);
            size += writer.Write(Unknown_05);
            size += writer.Write(SomeIterable);
            size += writer.Write(Unknown_07);
            size += writer.Write(Unknown_08);
            size += writer.Write(Unknown_10);

            size += writer.WriteInt64(DataSize);
            size += writer.WriteInt64(Unknown_18);

            if (SomeIterable != 0) size += writer.Write(Unknown_20);

            size += writer.WriteInt64(Unknown_LongA);
            size += writer.WriteInt64(Unknown_LongB);

            size += writer.Write(Data);
            size += writer.WriteUEString(Material);

            size += writer.Write(JFIFJunk);
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


    }

}
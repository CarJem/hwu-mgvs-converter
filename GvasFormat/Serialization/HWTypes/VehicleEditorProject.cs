using GvasFormat.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GvasFormat.Serialization.UETypes;

namespace GvasFormat.Serialization.HWTypes
{
    [DebuggerDisplay("{Vehicle}", Name = "{LiveryName}")]
    public class HWUVehicleEditorProject : UEStructProperty
    {

        public Guid Guid { get; set; }
        public string LiveryName { get; set; }
        public int TagCount { get; set; }
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

        public string Vehicle { get; set; }

        public List<UEProperty> Properties { get; set; } = new List<UEProperty>();

        public HWUVehicleEditorProject() { }
        public HWUVehicleEditorProject(BinaryReader reader, string name, string type, string structType, long valueLength)
        {
            Name = name;
            Type = type;
            StructType = structType;
            ValueLength = valueLength;

            Guid = new Guid(reader.ReadBytes(16));
            LiveryName = reader.ReadUEString();

            TagCount = reader.ReadInt32();

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
            while ((prop = Deserialize(reader)).Name != "None")
                Properties.Add(prop);

            Vehicle = reader.ReadUEString();
        }

        public override void SerializeStructProp(BinaryWriter writer)
        {
            writer.Write(Guid.ToByteArray());
            writer.WriteUEString(LiveryName);
            writer.WriteInt32(TagCount);

            for (int i = 0; i < TagCount; i++)
                writer.WriteUEString(Tags[i]);

            writer.WriteInt64(UnknownUUID);
            writer.Write(Unknown_00);
            writer.Write(Unknown_04);
            writer.Write(Unknown_05);
            writer.Write(SomeIterable);
            writer.Write(Unknown_07);
            writer.Write(Unknown_08);
            writer.Write(Unknown_10);

            writer.WriteInt64(DataSize);
            writer.WriteInt64(Unknown_18);

            if (SomeIterable != 0) writer.Write(Unknown_20);

            writer.WriteInt64(Unknown_LongA);
            writer.WriteInt64(Unknown_LongB);

            writer.Write(Data);
            writer.WriteUEString(Material);

            writer.Write(JFIFJunk);
            writer.Write(JFIFData);

            for (int i = 0; i < Properties.Count; i++)
                Properties[i].Serialize(writer);
            writer.WriteUENoneProperty();

            writer.WriteUEString(Vehicle);
        }


    }

}
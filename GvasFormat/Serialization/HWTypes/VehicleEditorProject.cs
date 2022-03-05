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

        public Int64 UnknownA { get; set; }
        public Int64 UnknownB { get; set; }
        public Int64 UnknownC { get; set; }
        public Int64 UnknownD { get; set; }

        public Int64 DataSize { get; set; }
        public Int64 UnknownE { get; set; }
        public Int64 DataSizeClone { get; set; }
        public Int64 UnknownF { get; set; }

        public byte[] Data { get; set; }

        public string Material { get; set; }

        public byte[] JFIFJunk { get; set; }
        public byte[] JFIFData { get; set; }

        public string Vehicle { get; set; }
        public Int32 UnknownG { get; set; }

        public List<UEProperty> Properties { get; set; } = new List<UEProperty>();

        public HWUVehicleEditorProject() { }
        public HWUVehicleEditorProject(BinaryReader reader)
        {
            Guid = new Guid(reader.ReadBytes(16));
            LiveryName = reader.ReadUEString();

            TagCount = reader.ReadInt32();

            for (int i = 0; i < TagCount; i++)
                Tags.Add(reader.ReadUEString());

            UnknownA = reader.ReadInt64();
            UnknownB = reader.ReadInt64();
            UnknownC = reader.ReadInt64();
            UnknownD = reader.ReadInt64();

            DataSize = reader.ReadInt64();
            UnknownE = reader.ReadInt64();
            DataSizeClone = reader.ReadInt64();
            UnknownF = reader.ReadInt64();
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

            writer.WriteInt64(UnknownA);
            writer.WriteInt64(UnknownB);
            writer.WriteInt64(UnknownC);
            writer.WriteInt64(UnknownD);

            writer.WriteInt64(DataSize);
            writer.WriteInt64(UnknownE);
            writer.WriteInt64(DataSizeClone);
            writer.WriteInt64(UnknownF);
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
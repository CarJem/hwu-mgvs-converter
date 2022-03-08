using GvasFormat.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GvasFormat.Serialization.UETypes;

namespace GvasFormat.Serialization.HWTypes
{
    public class HWULiveryRemoteGameData
    {
        public long HeaderUnknownA { get; set; }
        public int HeaderUnknownB { get; set; }
        public string Type { get; set; }
        public List<UEProperty> Properties { get; set; } = new List<UEProperty>();
        public List<string> ExtraStrings { get; set; } = new List<string>();
        public byte[] UnknownA { get; set; }
        public HWUVehicleEditorProject Project { get; set; }
        public List<string> PostExtraStrings { get; set; } = new List<string>();
        public List<UEProperty> PostProperties { get; set; } = new List<UEProperty>();
        public byte[] UnknownB { get; set; }


        public HWULiveryRemoteGameData() { }
        public HWULiveryRemoteGameData(BinaryReader reader)
        {
            HeaderUnknownA = reader.ReadInt64();
            HeaderUnknownB = reader.ReadInt32();
            reader.ReadTerminator();

            var itemOffset = reader.BaseStream.Position;
            Type = reader.ReadUEString();
            if (Type != "/Script/hotwheels.LiveryRemoteGameData")
                throw new FormatException($"Offset: 0x{itemOffset:x8}. Unknown value type '{Type}'");

            UEProperty prop;
            while ((prop = UEProperty.Deserialize(reader)).Name != UENoneProperty.PropertyName)
            {
                Properties.Add(prop);
                if (prop is HWUVehicleEditorProject)
                {
                    if (UEHomelessString.Exists(reader, UENoneProperty.PropertyName))
                        Properties.Add(new UEHomelessString());

                    if (UEHomelessString.Exists(reader, "Contest_01"))
                        Properties.Add(new UEHomelessString("Contest_01"));
                }
            }

            UnknownA = reader.ReadBytes(4);

            Project = new HWUVehicleEditorProject(reader, "", "StructProperty", "VehicleEditorProject", -1);

            if (UEHomelessString.Exists(reader, UENoneProperty.PropertyName))
                PostProperties.Add(new UEHomelessString());

            if (UEHomelessString.Exists(reader, "Contest_01"))
                PostProperties.Add(new UEHomelessString("Contest_01"));

            UnknownB = reader.ReadBytes(4);

        }

        public void SerializeProp(BinaryWriter writer)
        {
            writer.WriteInt64(HeaderUnknownA);
            writer.WriteInt32(HeaderUnknownB);
            writer.Write(false); //terminator

            writer.WriteUEString(Type);

            for (int i = 0; i < Properties.Count; i++)
                Properties[i].Serialize(writer);
            writer.WriteUENoneProperty();

            writer.Write(UnknownA);

            Project.SerializeStructProp(writer);

            for (int i = 0; i < PostProperties.Count; i++)
                PostProperties[i].SerializeProp(writer);

            writer.Write(UnknownB);
        }
    }

}
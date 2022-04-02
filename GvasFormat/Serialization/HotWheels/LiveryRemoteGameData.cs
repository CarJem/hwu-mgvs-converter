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
    public class LiveryRemoteGameData
    {
        public long HeaderUnknownA { get; set; }
        public int HeaderUnknownB { get; set; }
        public string Type { get; set; }
        public List<UEProperty> Properties { get; set; } = new List<UEProperty>();
        public byte[] UnknownA { get; set; }
        public VehicleEditorProject Project { get; set; }
        public byte[] UnknownB { get; set; }


        public LiveryRemoteGameData() { }
        public LiveryRemoteGameData(GvasReader reader)
        {
            HeaderUnknownA = reader.ReadInt64();
            HeaderUnknownB = reader.ReadInt32();
            reader.ReadTerminator();

            var itemOffset = reader.BaseStream.Position;
            Type = reader.ReadUEString();
            if (Type != "/Script/hotwheels.LiveryRemoteGameData")
                throw new FormatException($"Offset: 0x{itemOffset:x8}. Unknown value type '{Type}'");

            UEProperty prop;
            while ((prop = UESerializer.Deserialize(reader)).Name != UENoneProperty.PropertyName)
                Properties.Add(prop);

            UnknownA = reader.ReadBytes(4);

            Project = new VehicleEditorProject(reader, "", "StructProperty", "VehicleEditorProject", -1);

            UnknownB = reader.ReadBytes(4);

        }

        public long SerializeProp(GvasWriter writer)
        {
            long size = 0;

            size += writer.WriteInt64(HeaderUnknownA);
            size += writer.WriteInt32(HeaderUnknownB);
            size += writer.Write(false); //terminator

            size += writer.WriteUEString(Type);

            for (int i = 0; i < Properties.Count; i++)
                size += Properties[i].Serialize(writer);
            size += writer.WriteUENoneProperty();

            size += writer.Write(UnknownA);

            size += Project.SerializeStructProp(writer);

            size += writer.Write(UnknownB);

            return size;
        }
    }

}
using System.Collections.Generic;
using System.IO;
using GvasFormat.Serialization.UETypes;

namespace GvasFormat.Serialization.HWTypes
{
    public class HWUDownloadedLiveries : UEProperty
    {
        public const string PropertyType = "HWUDownloadedLiveries";
        public const string OriginalType = "ByteProperty";
        public const string OriginalName = "DownloadedLiveries";

        public int UnknownA { get; set; }
        public int Size { get; set; }
        public long HeaderUnknownA { get; set; }
        public int HeaderUnknownB { get; set; }
        public List<HWULiveryRemoteGameData> Items { get; set; } = new List<HWULiveryRemoteGameData>();

        public HWUDownloadedLiveries()
        {
            Type = PropertyType;
            Name = OriginalName;
        }

        public HWUDownloadedLiveries(BinaryReader reader, string name)
        {
            Type = PropertyType;
            Name = name;

            reader.ReadTerminator();
            UnknownA = reader.ReadInt32();
            Size = reader.ReadInt32();


            for (int i = 0; i < Size; i++)
                Items.Add(new HWULiveryRemoteGameData(reader));
        }
        public override void SerializeProp(BinaryWriter writer)
        {
            writer.Write(false); //terminator
            writer.WriteInt32(UnknownA);
            writer.WriteInt32(Size);

            for (int i = 0; i < Items.Count; i++)
                Items[i].SerializeProp(writer);
        }

        public override void SerializeMap(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }

}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GvasFormat.Serialization.UETypes;
using GvasFormat.Utils;

namespace GvasFormat.Serialization.HotWheels
{
    public class TileMarketingDownloadedTexture : UEStructProperty
    {
        public const string PropertyName = "TileMarketingDownloadedTexture";
        public static readonly byte[] PropertyBytes = new byte [] { 0x9C, 0x01, 0x00, 0x00, 0x70, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00 };

        public TileMarketingDownloadedTexture() { }
        public TileMarketingDownloadedTexture(BinaryReader reader, string name, string type, string structType, long valueLength) : base(name, type, structType, valueLength)
        {
            Header = reader.ReadBytes(16);
            var terminator = reader.ReadTerminator();
            Length = reader.ReadInt64();
            PNG_Data = reader.ReadPNG();
        }

        public static bool Exists(BinaryReader reader)
        {
            var initalPosition = reader.BaseStream.Position;
            var headerTest = reader.ReadBytes(16);

            reader.BaseStream.Position = initalPosition;
            return Enumerable.SequenceEqual(headerTest, PropertyBytes);
        }

        public override void SerializeStructProp(BinaryWriter writer)
        {   
            writer.Write(Header);
            writer.Write(false); //terminator
            writer.WriteInt64(Length);
            writer.Write(PNG_Data);
        }

        public byte[] Header { get; set; }
        public long Length { get; set; }
        public byte[] PNG_Data { get; set; }
    }
}

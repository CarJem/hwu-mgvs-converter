using System;
using System.IO;
using System.Linq;
using System.Text;
using GvasFormat.Serialization.UETypes;
using GvasFormat.Utils;

namespace GvasFormat.Serialization
{
    public static partial class UESerializer
    {
        public static Gvas Read(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                var header = reader.ReadBytes(Gvas.Header.Length);
                if (!Gvas.Header.SequenceEqual(header))
                    throw new FormatException($"Invalid header, expected {Gvas.Header}");

                var result = new Gvas();
                result.SaveGameVersion = reader.ReadInt32();
                result.PackageVersion = reader.ReadInt32();
                result.EngineVersion.Major = reader.ReadInt16();
                result.EngineVersion.Minor = reader.ReadInt16();
                result.EngineVersion.Patch = reader.ReadInt16();
                result.EngineVersion.Build = reader.ReadInt32();
                result.EngineVersion.BuildId = reader.ReadUEString();
                result.CustomFormatVersion = reader.ReadInt32();
                result.CustomFormatData.Count = reader.ReadInt32();
                result.CustomFormatData.Entries = new CustomFormatDataEntry[result.CustomFormatData.Count];
                for (var i = 0; i < result.CustomFormatData.Count; i++)
                {
                    var entry = new CustomFormatDataEntry();
                    entry.Id = new Guid(reader.ReadBytes(16));
                    entry.Value = reader.ReadInt32();
                    result.CustomFormatData.Entries[i] = entry;
                }
                result.SaveGameType = reader.ReadUEString();

                while (UEProperty.Deserialize(reader) is UEProperty prop)
                    result.Properties.Add(prop);

                result.Leftovers = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));

                return result;
            }
        }

        public static void Write(FileStream stream, Gvas data)
        {
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.ASCII, true))
            {
                writer.Write(Gvas.Header);

                writer.WriteInt32(data.SaveGameVersion);
                writer.WriteInt32(data.PackageVersion);
                writer.WriteInt16(data.EngineVersion.Major);
                writer.WriteInt16(data.EngineVersion.Minor);
                writer.WriteInt16(data.EngineVersion.Patch);
                writer.WriteInt32(data.EngineVersion.Build);
                writer.WriteUEString(data.EngineVersion.BuildId);
                writer.WriteInt32(data.CustomFormatVersion);
                writer.WriteInt32(data.CustomFormatData.Count);
                for (var i = 0; i < data.CustomFormatData.Count; i++)
                {
                    var entry = data.CustomFormatData.Entries[i];
                    writer.Write(entry.Id.ToByteArray());
                    writer.WriteInt32(entry.Value);
                }
                writer.WriteUEString(data.SaveGameType);

                foreach (UEProperty prop in data.Properties)
                {
                    prop.Serialize(writer);
                }

                writer.WriteInt32(0);
                writer.Write(data.Leftovers);
            }
        }
    }
}

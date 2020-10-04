using System;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    public abstract class UEStructProperty : UEProperty
    {
        public UEStructProperty() { }

        public static UEStructProperty Read(BinaryReader reader, long valueLength)
        {
            var type = reader.ReadUEString();
            var id = new Guid(reader.ReadBytes(16));
            if (id != Guid.Empty)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 16:x8}. Expected struct ID {Guid.Empty}, but was {id}");

            var terminator = reader.ReadByte();
            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            return ReadStructValue(type, reader, valueLength);
        }

        public static UEStructProperty[] Read(BinaryReader reader, long valueLength, int count)
        {
            var type = reader.ReadUEString();
            var id = new Guid(reader.ReadBytes(16));
            if (id != Guid.Empty)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 16:x8}. Expected struct ID {Guid.Empty}, but was {id}");

            var terminator = reader.ReadByte();
            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            var result = new UEStructProperty[Math.Max(count, 1)];
            if (count == 0)
            {
                result[0] = new UEGenericStructProperty();
                result[0].StructType = type;
                return result;
            }
            for (var i = 0; i < count; i++)
                result[i] = ReadStructValue(type, reader, valueLength);
            return result;
        }

        protected static UEStructProperty ReadStructValue(string type, BinaryReader reader, long valueLength)
        {
            UEStructProperty result;
            switch (type)
            {
                case "DateTime":
                    result = new UEDateTimeStructProperty(reader);
                    break;
                case "Guid":
                    result = new UEGuidStructProperty(reader);
                    break;
                case "Vector":
                case "Rotator":
                    result = new UEVectorStructProperty(reader);
                    break;
                case "LinearColor":
                    result = new UELinearColorStructProperty(reader);
                    break;
                /*case "Transform":
                    result = new UETransformStructProperty(reader);
                    break;*/
                case "Quat":
                    result = new UEQuaternionStructProperty(reader);
                    break;
                default:
                    result = new UEGenericStructProperty(reader);
                    break;
            }
            result.StructType = type;
            result.ValueLength = valueLength;
            return result;
        }

        public abstract void SerializeStructProp(BinaryWriter writer);

        public override void SerializeProp(BinaryWriter writer)
        {
            writer.WriteUEString(StructType);
            writer.Write(Guid.Empty.ToByteArray());
            writer.Write(false);
            SerializeStructProp(writer);
        }

        public string StructType;
        //public Guid Unknown = Guid.Empty;
    }
}
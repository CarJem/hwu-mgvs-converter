using System;
using System.IO;
using System.Linq;

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

            var result = UESerializer.DeserializeStruct(type, valueLength, reader);
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
    }
}
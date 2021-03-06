using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    public abstract class UEStructProperty : UEProperty
    {
        public UEStructProperty() { }
        public UEStructProperty(string name, string type, string structType, long valueLength) : base(name, type, valueLength) 
        {
            StructType = structType;
        }

        public static UEStructProperty Read(GvasReader reader, string name, string type, long valueLength)
        {
            var structType = reader.ReadUEString();

            var id = new Guid(reader.ReadBytes(16));
            if (id != Guid.Empty)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 16:x8}. Expected struct ID {Guid.Empty}, but was {id}");

            var terminator = reader.ReadByte();
            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            var result = UESerializer.DeserializeStruct(name, type, structType, valueLength, reader);
            return result;
        }

        public abstract long SerializeStructProp(GvasWriter writer);

        public override long SerializeProp(GvasWriter writer)
        {
            if (UESerializer.IsHWUDirectSeralizableStructureProperty(StructType))
            {
                return SerializeStructProp(writer);
            } 
            else
            {
                long size = 0;
                writer.WriteUEString(StructType);
                writer.Write(Guid.Empty.ToByteArray());
                writer.Write(false);
                size += SerializeStructProp(writer);
                return size;
            }

        }

        public string StructType;
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GvasFormat.Serialization.UETypes
{
    public abstract class UEStructProperty : UEProperty
    {
        public UEStructProperty() { }
        public UEStructProperty(string name, string type, string structType, long valueLength) : base(name, type, valueLength) 
        {
            StructType = structType;
        }

        public static UEStructProperty Read(BinaryReader reader, string name, string type, long valueLength)
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

        public abstract void SerializeStructProp(BinaryWriter writer);

        public override void SerializeMap(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void SerializeProp(BinaryWriter writer)
        {
            writer.WriteUEString(StructType);
            writer.Write(Guid.Empty.ToByteArray());
            writer.Write(false);
            SerializeStructProp(writer);
        }

        public static UEProperty[] DeserializeArray(BinaryReader reader, string name, string type, string arrayType, long valueLength)
        {
            var array = new List<UEProperty>();

            var terminator = reader.ReadTerminator();
            var arrayLength = reader.ReadInt32();
            for (int i = 0; i < arrayLength; i++)
            {
                if (i == 0) array.Add(UEProperty.Deserialize(reader));
                else
                {
                    var structType = (array[0] as UEStructProperty).StructType;
                    var item = UESerializer.DeserializeStruct(name, type, structType, arrayLength, reader);
                    item.Name = array[0].Name;
                    item.Type = array[0].Type;
                    array.Add(item);

                    if (UEHomelessString.Exists(reader)) array.Add(new UEHomelessString());
                }
            }

            return array.ToArray();
        }

        public static void SerializeArray(BinaryWriter writer, UEProperty[] Items)
        {
            writer.Write(false); //terminator
            writer.WriteInt32(Items.Length);
            for (int i = 0; i < Items.Length; i++)
            {
                UEProperty prop = Items[i];
                if (i == 0) { prop.Serialize(writer); }
                else if (prop is UEStructProperty) { ((UEStructProperty)prop).SerializeStructProp(writer); }
                else { prop.SerializeProp(writer); }
            }
        }

        public string StructType;
    }
}
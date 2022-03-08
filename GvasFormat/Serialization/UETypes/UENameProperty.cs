using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UENameProperty : UEProperty
    {
        private static readonly Encoding Utf8 = new UTF8Encoding(false);

        public UENameProperty() { }
        public UENameProperty(BinaryReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            if (valueLength > -1)
            {
                var terminator = reader.ReadByte();
                if (terminator != 0)
                    throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");
                Value = reader.ReadUEStringProperty(valueLength);
            } else
            {
                Value = reader.ReadUEString();
            }

        }
        public override void SerializeMap(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void SerializeProp(BinaryWriter writer, bool bodyOnly = false)
        {
            if (!bodyOnly) writer.Write(false); //terminator
            writer.WriteUEString(Value);
        }

        public override void SerializeProp(BinaryWriter writer)
        {
            SerializeProp(writer, false);
        }

        public static UEProperty[] DeserializeArray(BinaryReader reader, string name, string type, string arrayType, long valueLength)
        {
            List<UEProperty> array = new List<UEProperty>();
            var terminator = reader.ReadTerminator();
            var arrayLength = reader.ReadInt32();
            for (int i = 0; i < arrayLength; i++)
            {
                var item = UESerializer.DeserializeProperty(null, arrayType, -1, reader);
                array.Add(item);
            }
            return array.ToArray();
        }

        public static void SerializeArray(BinaryWriter writer, UEProperty[] Items)
        {
            writer.Write(false); //terminator
            writer.WriteInt32(Items.Length);
            for (int i = 0; i < Items.Length; i++)
            {
                UENameProperty prop = Items[i] as UENameProperty;
                prop.SerializeProp(writer, true);
            }
        }

        public string Value;
    }
}
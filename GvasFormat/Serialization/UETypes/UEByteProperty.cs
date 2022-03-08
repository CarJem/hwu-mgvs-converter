using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using GvasFormat.Utils;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEByteProperty : UEProperty
    {
        public UEByteProperty() { }
        public UEByteProperty(BinaryReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            var terminator = reader.ReadTerminator();
            var arrayLength = reader.ReadInt32();
            Value = reader.ReadBytes(arrayLength);
        }
        public override void SerializeMap(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void SerializeProp(BinaryWriter writer)
        {
            writer.Write(false); //terminator
            writer.WriteInt32(Value.Length);
            writer.Write(Value);
        }

        public static UEProperty[] DeserializeArray(BinaryReader reader, string name, string type, string arrayType, long valueLength)
        {
            List<UEProperty> array = new List<UEProperty>();
            array.Add(UESerializer.DeserializeProperty(null, arrayType, valueLength, reader));
            return array.ToArray();
        }

        public static void SerializeArray(BinaryWriter writer, UEProperty[] Items)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                UEProperty prop = Items[i];
                if (i == 0) { prop.Serialize(writer); }
                else { prop.SerializeProp(writer); }
            }
        }

        public byte[] Value;
    }
}
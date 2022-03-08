using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GvasFormat.Utils;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("Count = {Map.Count}", Name = "{Name}")]
    public sealed class UESetProperty : UEProperty
    {
        public UESetProperty() { }
        public UESetProperty(BinaryReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            ValueType = reader.ReadUEString();
            var terminator = reader.ReadBytes(5);
            SetLength = reader.ReadInt32();
            for (int i = 0; i < SetLength; ++i)
                Properties.Add(UESerializer.DeserializeProperty("", ValueType, -1, reader));
        }

        public override void SerializeProp(BinaryWriter writer)
        {
            writer.WriteUEString(ValueType);
            writer.Write(new byte[5]); //terminator
            writer.WriteInt32(SetLength);
            for (int i = 0; i < SetLength; ++i)
                Properties[i].SerializeProp(writer);

        }

        public List<UEProperty> Properties = new List<UEProperty>();
        public int SetLength;
        public string ValueType;
    }
}
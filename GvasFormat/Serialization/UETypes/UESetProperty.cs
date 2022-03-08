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
            SetLength = reader.ReadInt32();
            var terminator = reader.ReadByte();
            ValueType = reader.ReadUEString();

        }
        public override void SerializeMap(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void SerializeProp(BinaryWriter writer)
        {
            writer.WriteInt32(SetLength);
            writer.Write(false); //terminator
            writer.WriteUEString(ValueType);
        }

        public int SetLength;
        public string ValueType;
    }
}
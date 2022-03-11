using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("Count = {Map.Count}", Name = "{Name}")]
    public sealed class UESetProperty : UEProperty
    {
        public UESetProperty() { }
        public UESetProperty(GvasReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            ValueType = reader.ReadUEString();
            var terminator = reader.ReadTerminator();
            var blank = reader.ReadBytes(4);
            SetLength = reader.ReadInt32();
            for (int i = 0; i < SetLength; ++i)
                Properties.Add(UESerializer.DeserializeProperty("", ValueType, -1, reader));
        }

        public override long SerializeProp(GvasWriter writer)
        {
            long size = 0;
            writer.WriteUEString(ValueType);
            writer.Write(false); //terminator
            size += writer.Write(new byte[4]); //blank
            size += writer.WriteInt32(SetLength);
            for (int i = 0; i < SetLength; ++i)
                size += Properties[i].SerializeProp(writer);
            return size;
        }

        public List<UEProperty> Properties = new List<UEProperty>();
        public int SetLength;
        public string ValueType;
    }
}
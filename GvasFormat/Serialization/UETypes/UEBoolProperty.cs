using System;
using System.Diagnostics;
using System.IO;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEBoolProperty : UEProperty
    {
        public UEBoolProperty() { }
        public UEBoolProperty(GvasReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            if (valueLength != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected bool value length 0, but was {valueLength}");

            var val = reader.ReadInt16();
            if (val == 0)
                Value = false;
            else if (val == 1)
                Value = true;
            else
                throw new InvalidOperationException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected bool value, but was {val}");
        }

        public override long SerializeProp(GvasWriter writer)
        {
            long size = 0;
            writer.WriteInt16((short)(Value ? 1 : 0));
            return size;
        }

        public bool Value;
    }
}
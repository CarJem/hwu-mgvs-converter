using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEObjectProperty : UEProperty
    {
        public UEObjectProperty() { }
        public UEObjectProperty(BinaryReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            var terminator = reader.ReadByte();
            Value = reader.ReadBytes((int)valueLength);
        }
        public override void SerializeMap(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void SerializeProp(BinaryWriter writer)
        {
            writer.Write(false); //terminator
            writer.Write(Value);
        }

        public byte[] Value;
    }
}
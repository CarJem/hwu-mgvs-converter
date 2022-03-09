using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public sealed class UEEnumProperty : UEProperty
    {
        private static readonly Encoding Utf8 = new UTF8Encoding(false);

        public UEEnumProperty() { }
        public UEEnumProperty(BinaryReader reader, string name, string type, long valueLength) : base(name, type, valueLength)
        {
            EnumType = reader.ReadUEString();

            if (EnumType.Contains("::"))
            {
                IsCompactName = true;
            }
            else
            {
                IsCompactName = false;
                reader.ReadTerminator();
                Value = reader.ReadUEString();
            }
        }

        public override void SerializeProp(BinaryWriter writer)
        {
            if (IsCompactName)
            {
                writer.WriteUEString(EnumType);
            }
            else
            {
                writer.WriteUEString(EnumType);
                writer.Write(false); //terminator
                writer.WriteUEString(Value);
            }


        }

        public bool IsCompactName;
        public string EnumType;
        public string Value;
    }
}
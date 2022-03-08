using System;
using System.Diagnostics;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("", Name = "{Name}")]
    public sealed class UEHomelessString : UEProperty
    {
        public const string PropertyName = "Blank";
        public string LegitName { get; set; }
        public UEHomelessString()
        {
            Name = PropertyName;
            LegitName = "None";
        }

        public UEHomelessString(string name)
        {
            Name = PropertyName;
            LegitName = name;
        }

        public override void SerializeProp(BinaryWriter writer)
        {
            writer.WriteInt32(1);
            writer.WriteUEString(LegitName);
        }

        public override void SerializeMap(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public static bool Exists(BinaryReader reader, string desiredName = "None")
        {
            var initalPosition = reader.BaseStream.Position;

            var emptyString = reader.ReadUEString(1);
            var noneString = reader.ReadUEString(desiredName.Length + 1);

            if (emptyString != "" || noneString != desiredName)
            {
                reader.BaseStream.Position = initalPosition;
                return false;
            }
            else return true;
        }
    }
}
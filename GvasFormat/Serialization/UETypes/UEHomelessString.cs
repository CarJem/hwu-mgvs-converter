using System;
using System.Diagnostics;
using System.IO;
using GvasFormat.Utils;
using GvasFormat.Serializer;

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
            LegitName = UENoneProperty.PropertyName;
        }

        public UEHomelessString(string name)
        {
            Name = PropertyName;
            LegitName = name;
        }

        public override long SerializeProp(GvasWriter writer)
        {
            long size = 0;
            size += writer.WriteInt32(1);
            size += writer.WriteUEString(LegitName);
            return size;
        }

        public static bool Exists(GvasReader reader, string desiredName = UENoneProperty.PropertyName)
        {
            var initalPosition = reader.BaseStream.Position;

            var emptyString = reader.ReadUEString();
            var noneString = reader.ReadUEString();

            if (emptyString != "" || noneString != desiredName)
            {
                reader.BaseStream.Position = initalPosition;
                return false;
            }
            else return true;
        }
    }
}
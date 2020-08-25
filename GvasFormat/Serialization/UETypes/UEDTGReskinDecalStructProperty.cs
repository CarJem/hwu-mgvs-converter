using System;
using System.Collections.Generic;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    internal class UEDTGReskinDecalStructProperty : UEStructProperty
    {
        public List<UEProperty> ModelTransforms = new List<UEProperty>();

        public UEDTGReskinDecalStructProperty(BinaryReader reader)
        {
            //int terminator = reader.ReadByte();
            //if (terminator != 0)
            //    throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            //long unknown0 = reader.ReadInt32();
            //long unknown1 = reader.ReadInt32();
            //long unknown2 = reader.ReadInt32();
            //long unknown3 = reader.ReadInt32();

            while (Read(reader) is UEProperty prop)
            {
                ModelTransforms.Add(prop);
            }
            
        }
    }
}
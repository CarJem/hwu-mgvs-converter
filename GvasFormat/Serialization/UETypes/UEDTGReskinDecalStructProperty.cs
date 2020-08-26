using System;
using System.Collections.Generic;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    internal class UEDTGReskinDecalStructProperty : UEStructProperty
    {
        public List<UEStructProperty> ModelTransforms = new List<UEStructProperty>();

        public UEDTGReskinDecalStructProperty(BinaryReader reader)
        {
            while (Read(reader) is UEStructProperty prop)
            {
                ModelTransforms.Add(prop);
            }
            
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.WriteUEString(Name);
            writer.WriteUEString(Type);
            writer.WriteInt64(0); //valueLength
            writer.WriteUEString(StructType);

            foreach (UEStructProperty modelTransform in ModelTransforms)
            {
                modelTransform.Serialize(writer);
            }
        }
    }
}
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("Count = {Properties.Count}", Name = "{Name}")]
    public sealed class UEGenericStructProperty : UEStructProperty
    {
        public List<UEProperty> Properties = new List<UEProperty>();

        public override void Serialize(BinaryWriter writer)
        {
            writer.WriteUEString(Name);
            writer.WriteUEString(Type);
            writer.WriteInt64(0); //valueLength
            writer.WriteUEString(StructType);

            foreach (UEProperty prop in Properties)
            {
                prop.Serialize(writer);
            }
        }
    }
}
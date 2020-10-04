using System.Diagnostics;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("", Name = "{Name}")]
    public sealed class UENoneProperty : UEProperty
    {

        public UENoneProperty() { Name = "None"; }

        public override void SerializeProp(BinaryWriter writer)
        {
            long pos = writer.BaseStream.Position;
            writer.WriteUEString("None");
        }
    }
}
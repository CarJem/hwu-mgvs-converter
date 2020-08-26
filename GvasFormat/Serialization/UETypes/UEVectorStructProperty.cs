using System.Diagnostics;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("X = {X}, Y = {Y}, Z = {Z}", Name = "{Name}")]
    public sealed class UEVectorStructProperty : UEStructProperty
    {
        public UEVectorStructProperty() { }
        public UEVectorStructProperty(BinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }

        public float X, Y, Z;

        public override void Serialize(BinaryWriter writer)
        {
            writer.WriteUEString(Name);
            writer.WriteUEString(Type);
            writer.WriteInt64(0); // valueLength
            writer.WriteUEString(StructType);
            writer.WriteSingle(X);
            writer.WriteSingle(Y);
            writer.WriteSingle(Z);
        }
    }
}
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    internal class UEQuaternionStructProperty : UEStructProperty
    {

        public UEQuaternionStructProperty(BinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            I = reader.ReadSingle();
            J = reader.ReadSingle();
        }

        public float X;
        public float Y;
        public float I;
        public float J;

        public override void Serialize(BinaryWriter writer)
        {
            writer.WriteUEString(Name);
            writer.WriteUEString(Type);
            writer.WriteInt64(0); //valueLength
            writer.WriteUEString(StructType);
            writer.WriteSingle(X);
            writer.WriteSingle(Y);
            writer.WriteSingle(I);
            writer.WriteSingle(J);
        }
    }
}
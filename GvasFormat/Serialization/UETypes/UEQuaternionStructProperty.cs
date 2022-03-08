using System;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    internal class UEQuaternionStructProperty : UEStructProperty
    {

        public UEQuaternionStructProperty(BinaryReader reader, string name, string type, string structType, long valueLength)
        {
            Name = name;
            Type = type;
            StructType = structType;
            ValueLength = valueLength;

            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            I = reader.ReadSingle();
            J = reader.ReadSingle();
        }

        public UEQuaternionStructProperty()
        {
        }

        public float X;
        public float Y;
        public float I;
        public float J;

        public override void SerializeStructProp(BinaryWriter writer)
        {
            writer.WriteSingle(X);
            writer.WriteSingle(Y);
            writer.WriteSingle(I);
            writer.WriteSingle(J);
        }
    }
}
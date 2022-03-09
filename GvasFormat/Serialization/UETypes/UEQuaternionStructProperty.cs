using System;
using System.IO;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    internal class UEQuaternionStructProperty : UEStructProperty
    {

        public UEQuaternionStructProperty(BinaryReader reader, string name, string type, string structType, long valueLength) : base(name, type, structType, valueLength)
        {
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
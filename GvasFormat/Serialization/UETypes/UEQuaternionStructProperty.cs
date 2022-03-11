using System;
using System.IO;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    internal class UEQuaternionStructProperty : UEStructProperty
    {

        public UEQuaternionStructProperty(GvasReader reader, string name, string type, string structType, long valueLength) : base(name, type, structType, valueLength)
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

        public override long SerializeStructProp(GvasWriter writer)
        {
            long size = 0;
            size += writer.WriteSingle(X);
            size += writer.WriteSingle(Y);
            size += writer.WriteSingle(I);
            size += writer.WriteSingle(J);
            return size;
        }
    }
}
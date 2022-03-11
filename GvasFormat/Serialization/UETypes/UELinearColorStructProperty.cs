using System;
using System.Diagnostics;
using System.IO;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("R = {R}, G = {G}, B = {B}, A = {A}", Name = "{Name}")]
    public sealed class UELinearColorStructProperty : UEStructProperty
    {
        public UELinearColorStructProperty() { }
        public UELinearColorStructProperty(GvasReader reader, string name, string type, string structType, long valueLength) : base(name, type, structType, valueLength)
        {
            ValueLength = valueLength;
            R = reader.ReadSingle();
            G = reader.ReadSingle();
            B = reader.ReadSingle();
            A = reader.ReadSingle();
        }

        public float R, G, B, A;

        public override long SerializeStructProp(GvasWriter writer)
        {
            long size = 0;
            if (ValueLength == 0) return size;
            size += writer.WriteSingle(R);
            size += writer.WriteSingle(G);
            size += writer.WriteSingle(B);
            size += writer.WriteSingle(A);
            return size;
        }

        public long ValueLength;
    }
}
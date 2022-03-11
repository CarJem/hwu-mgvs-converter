using System;
using System.Diagnostics;
using System.IO;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("X = {X}, Y = {Y}, Z = {Z}", Name = "{Name}")]
    public sealed class UEVectorStructProperty : UEStructProperty
    {
        public UEVectorStructProperty() { }
        public UEVectorStructProperty(GvasReader reader, string name, string type, string structType, long valueLength) : base(name, type, structType, valueLength)
        {
            StructType = structType;

            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }

        public float X, Y, Z;

        public override long SerializeStructProp(GvasWriter writer)
        {
            long size = 0;
            size += writer.WriteSingle(X);
            size += writer.WriteSingle(Y);
            size += writer.WriteSingle(Z);
            return size;
        }
    }
}
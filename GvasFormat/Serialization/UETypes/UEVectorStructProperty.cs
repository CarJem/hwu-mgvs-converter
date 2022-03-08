﻿using System;
using System.Diagnostics;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("X = {X}, Y = {Y}, Z = {Z}", Name = "{Name}")]
    public sealed class UEVectorStructProperty : UEStructProperty
    {
        public UEVectorStructProperty() { }
        public UEVectorStructProperty(BinaryReader reader, string name, string type, string structType, long valueLength) : base(name, type, structType, valueLength)
        {
            StructType = structType;

            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }

        public float X, Y, Z;

        public override void SerializeStructProp(BinaryWriter writer)
        {
            writer.WriteSingle(X);
            writer.WriteSingle(Y);
            writer.WriteSingle(Z);
        }
    }
}
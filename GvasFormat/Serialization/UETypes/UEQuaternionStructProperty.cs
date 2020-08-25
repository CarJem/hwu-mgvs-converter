using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    internal class UEQuaternionStructProperty : UEStructProperty
    {

        public UEQuaternionStructProperty(BinaryReader reader)
        {
            X = reader.ReadInt32();
            Y = reader.ReadInt32();
            I = reader.ReadInt32();
            J = reader.ReadInt32();
        }

        public long X;
        public long Y;
        public long I;
        public long J;
    }
}
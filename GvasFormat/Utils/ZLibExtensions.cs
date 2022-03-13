using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GvasFormat.Utils
{
    public static class ZLibExtensions
    {
        public static byte[] Decompress(byte[] data)
        {
            if (data == null) return null;

            byte[] output;

            using (var outStream = new MemoryStream(data.Length * 2))
            using (var inStream = new MemoryStream(data))
            using (var zlib = new ZlibStream(inStream, CompressionMode.Decompress))
            {
                CopyStream(zlib, outStream);
                output = outStream.ToArray();
            }
            return output;
        }
        public static byte[] Compress(byte[] data)
        {
            if (data == null) return null;

            byte[] output;

            using (var outStream = new MemoryStream(data.Length))
            {
                using (var zlib = new ZlibStream(outStream, CompressionMode.Compress))
                {
                    zlib.Write(data, 0, data.Length);
                }
                output = outStream.ToArray();
            }
            return output;
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }
    }
}

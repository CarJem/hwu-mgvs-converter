using System;
using System.Collections.Generic;
using System.Text;

namespace GvasFormat.Utils
{
    public static class HexExtensions
    {
        public static string ToHexString(byte[] value)
        {
            return BitConverter.ToString(value).Replace("-", string.Empty);
        }
        public static byte[] FromHexString(string hexString)
        {
            byte[] retval = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i += 2)
                retval[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            return retval;
        }
    }
}

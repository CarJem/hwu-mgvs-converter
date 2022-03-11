using GvasFormat.Serialization.UETypes;
using GvasFormat.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GvasFormat.Utils
{
    public class GvasWriter : IDisposable
    {
        private static readonly Encoding Utf8 = new UTF8Encoding(false);

        private BinaryWriter Instance;

        public GvasWriter(Stream output)
        {
            Instance = new BinaryWriter(output);
        }

        public GvasWriter(Stream output, Encoding encoding)
        {
            Instance = new BinaryWriter(output, encoding);
        }

        public GvasWriter(Stream output, Encoding encoding, bool leaveOpen)
        {
            Instance = new BinaryWriter(output, encoding, leaveOpen);
        }

        public Stream BaseStream => Instance.BaseStream;


        #region Write

        public long WriteUENoneProperty()
        {
            return WriteUEString(UENoneProperty.PropertyName);
        }
        public long WriteUEString(string value)
        {
            long size = 0;

            if (value == null)
            {
                size += Write(0);
                return size;
            }

            var valueBytes = Utf8.GetBytes(value);
            size += Write(valueBytes.Length + 1);
            if (valueBytes.Length > 0)
                size += Write(valueBytes);
            size += Write((byte)0);
            return size;
        }
        public long WriteUEString(string value, long vl)
        {
            long size = 0;

            if (value == null)
            {
                size += Write(0);
                return size;
            }

            var valueBytes = Utf8.GetBytes(value);
            size += Write(valueBytes.Length + 1);
            if (valueBytes.Length > 0)
                size += Write(valueBytes);
            size += Write(false);
            while (vl > valueBytes.Length + 5)
            {
                size += Write(false);
                vl--;
            }
            return size;
        }
        public long WriteInt64(long value)
        {
            return Write(BitConverter.GetBytes(value));
        }
        public long WriteInt32(int value)
        {
            return Write(BitConverter.GetBytes(value));
        }
        public long WriteInt16(short value)
        {
            return Write(BitConverter.GetBytes(value));
        }
        public long WriteSingle(float value)
        {
            return Write(BitConverter.GetBytes(value));
        }
        public long WriteDouble(double value)
        {
            return Write(BitConverter.GetBytes(value));
        }

        #endregion

        #region Methods

        public void Close()
        {
            Instance.Close();
        }

        public void Dispose()
        {
            Instance.Dispose();
        }

        public void Flush()
        {
            Instance.Flush();
        }

        #endregion

        #region Writing

        public long Seek(int offset, SeekOrigin origin)
        {
            return Instance.Seek(offset, origin);
        }

        public long Write(bool value)
        {
            Instance.Write(value);
            return 1;
        }

        public long Write(byte value)
        {
            Instance.Write(value);
            return 1;
        }

        public long Write(byte[] buffer)
        {
            Instance.Write(buffer);
            return buffer.Length;
        }

        public long Write(byte[] buffer, int index, int count)
        {
            Instance.Write(buffer, index, count);
            return count;
        }

        public long Write(char ch)
        {
            Instance.Write(ch);
            return 2;
        }

        public long Write(char[] chars)
        {
            Instance.Write(chars);
            return (chars.Length - 1) * 2;
        }

        public long Write(char[] chars, int index, int count)
        {
            Instance.Write(chars, index, count);
            return count * 2;
        }

        public long Write(decimal value)
        {
            Instance.Write(value);
            return 16;
        }

        public long Write(double value)
        {
            Instance.Write(value);
            return 8;
        }

        public long Write(short value)
        {
            Instance.Write(value);
            return 2;
        }

        public long Write(int value)
        {
            Instance.Write(value);
            return 4;
        }

        public long Write(long value)
        {
            Instance.Write(value);
            return 8;
        }

        public long Write(sbyte value)
        {
            Instance.Write(value);
            return 1;

        }

        public long Write(float value)
        {
            Instance.Write(value);
            return 4;
        }

        public long Write(ushort value)
        {
            Instance.Write(value);
            return 2;
        }

        public long Write(uint value)
        {
            Instance.Write(value);
            return 4;
        }

        public long Write(ulong value)
        {
            Instance.Write(value);
            return 8;
        }

        #endregion
    }
}

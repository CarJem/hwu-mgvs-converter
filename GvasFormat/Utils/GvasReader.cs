using GvasFormat.Serialization.UETypes;
using GvasFormat.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GvasFormat.Utils
{
    public class GvasReader : BinaryReader
    {
        private static readonly Encoding Utf8 = new UTF8Encoding(false);

        public GvasReader(Stream input) : base(input)
        {
        }

        public GvasReader(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        public GvasReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        public override Stream BaseStream => base.BaseStream;

        #region Read

        public string ReadUEString()
        {
            if (PeekChar() < 0)
                return null;

            var length = ReadInt32();
            if (length <= 0 || length >= 512)
                return null;

            if (length == 1)
                return "";

            var valueBytes = ReadBytes(length);
            return Utf8.GetString(valueBytes, 0, valueBytes.Length - 1);
        }

        public string ReadUEString(long vl)
        {
            if (PeekChar() < 0)
                return null;

            var length = ReadInt32();
            if (length == 0)
                return null;

            if (length == 1)
                return "";

            var valueBytes = ReadBytes((int)vl - 4);
            return Utf8.GetString(valueBytes, 0, length - 1);
        }

        public byte[] ReadPNG()
        {
            List<byte> data = new List<byte>();
            List<byte> end = new List<byte>() { 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82 };
            bool isFinished = false;
            while (!isFinished)
            {
                data.Add(ReadByte());
                if (data.Count >= 8)
                {
                    var trimmed = data.TakeLast<byte>(8).ToList();

                    if (Enumerable.SequenceEqual(trimmed, end)) isFinished = true;
                }
            }
            return data.ToArray();
        }

        public byte[] ReadJFIF()
        {
            List<byte> data = new List<byte>();
            bool isFinished = false;
            while (!isFinished)
            {
                data.Add(ReadByte());
                if (data.Count >= 2)
                {
                    var trimmed = data.TakeLast<byte>(2).ToList();
                    if (trimmed[0] == 0xFF && trimmed[1] == 0xD9) isFinished = true;
                }
            }
            return data.ToArray();
        }

        public byte ReadTerminator()
        {
            var terminator = ReadByte();
            if (terminator != 0)
                throw new FormatException($"Offset: 0x{BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");
            return terminator;
        }

        #endregion

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override void Close()
        {
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void FillBuffer(int numBytes)
        {
            base.FillBuffer(numBytes);
        }

        public override int PeekChar()
        {
            return base.PeekChar();
        }

        public override int Read()
        {
            return base.Read();
        }

        public override int Read(byte[] buffer, int index, int count)
        {
            return base.Read(buffer, index, count);
        }

        public override int Read(char[] buffer, int index, int count)
        {
            return base.Read(buffer, index, count);
        }

        public override bool ReadBoolean()
        {
            return base.ReadBoolean();
        }

        public override byte ReadByte()
        {
            return base.ReadByte();
        }

        public override byte[] ReadBytes(int count)
        {
            return base.ReadBytes(count);
        }

        public override char ReadChar()
        {
            return base.ReadChar();
        }

        public override char[] ReadChars(int count)
        {
            return base.ReadChars(count);
        }

        public override decimal ReadDecimal()
        {
            return base.ReadDecimal();
        }

        public override double ReadDouble()
        {
            return base.ReadDouble();
        }

        public override short ReadInt16()
        {
            return base.ReadInt16();
        }

        public override int ReadInt32()
        {
            return base.ReadInt32();
        }

        public override long ReadInt64()
        {
            return base.ReadInt64();
        }

        public override sbyte ReadSByte()
        {
            return base.ReadSByte();
        }

        public override float ReadSingle()
        {
            return base.ReadSingle();
        }

        public override string ReadString()
        {
            return base.ReadString();
        }

        public override ushort ReadUInt16()
        {
            return base.ReadUInt16();
        }

        public override uint ReadUInt32()
        {
            return base.ReadUInt32();
        }

        public override ulong ReadUInt64()
        {
            return base.ReadUInt64();
        }
    }
}

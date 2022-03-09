using GvasFormat.Serialization.UETypes;
using GvasFormat.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GvasFormat.Utils
{
    public static class BinaryReaderEx
    {
        private static readonly Encoding Utf8 = new UTF8Encoding(false);

        #region Read

        public static string ReadUEString(this BinaryReader reader)
        {
            if (reader.PeekChar() < 0)
                return null;

            var length = reader.ReadInt32();
            if (length <= 0 || length >= 512)
                return null;

            if (length == 1)
                return "";

            var valueBytes = reader.ReadBytes(length);
            return Utf8.GetString(valueBytes, 0, valueBytes.Length - 1);
        }

        public static string ReadUEString(this BinaryReader reader, long vl)
        {
            if (reader.PeekChar() < 0)
                return null;

            var length = reader.ReadInt32();
            if (length == 0)
                return null;

            if (length == 1)
                return "";

            var valueBytes = reader.ReadBytes((int)vl - 4);
            return Utf8.GetString(valueBytes, 0, length - 1);
        }

        public static byte[] ReadPNG(this BinaryReader reader)
        {
            List<byte> data = new List<byte>();
            List<byte> end = new List<byte>() { 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82 };
            bool isFinished = false;
            while (!isFinished)
            {
                data.Add(reader.ReadByte());
                if (data.Count >= 8)
                {
                    var trimmed = data.TakeLast<byte>(8).ToList();

                    if (Enumerable.SequenceEqual(trimmed, end)) isFinished = true;
                }
            }
            return data.ToArray();
        }

        public static byte[] ReadJFIF(this BinaryReader reader)
        {
            List<byte> data = new List<byte>();
            bool isFinished = false;
            while (!isFinished)
            {
                data.Add(reader.ReadByte());
                if (data.Count >= 2)
                {
                    var trimmed = data.TakeLast<byte>(2).ToList();
                    if (trimmed[0] == 0xFF && trimmed[1] == 0xD9) isFinished = true;
                }
            }
            return data.ToArray();
        }

        public static byte ReadTerminator(this BinaryReader reader)
        {
            var terminator = reader.ReadByte();
            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");
            return terminator;
        }

        #endregion

        #region Write

        public static void WriteUENoneProperty(this BinaryWriter writer)
        {
            writer.WriteUEString(UENoneProperty.PropertyName);
        }
        public static void WriteUEString(this BinaryWriter writer, string value)
        {
            if (value == null)
            {
                writer.Write(0);
                return;
            }

            var valueBytes = Utf8.GetBytes(value);
            writer.Write(valueBytes.Length + 1);
            if (valueBytes.Length > 0)
                writer.Write(valueBytes);
            writer.Write((byte)0);
        }
        public static void WriteUEString(this BinaryWriter writer, string value, long vl)
        {
            if (value == null)
            {
                writer.Write(0);
                return;
            }

            var valueBytes = Utf8.GetBytes(value);
            writer.Write(valueBytes.Length + 1);
            if (valueBytes.Length > 0)
                writer.Write(valueBytes);
            writer.Write(false);
            while (vl > valueBytes.Length + 5)
            {
                writer.Write(false);
                vl--;
            }
        }
        public static void WriteInt64(this BinaryWriter writer, long value)
        {
            writer.Write(BitConverter.GetBytes(value));
        }
        public static void WriteInt32(this BinaryWriter writer, int value)
        {
            writer.Write(BitConverter.GetBytes(value));
        }
        public static void WriteInt16(this BinaryWriter writer, short value)
        {
            writer.Write(BitConverter.GetBytes(value));
        }
        public static void WriteSingle(this BinaryWriter writer, float value)
        {
            writer.Write(BitConverter.GetBytes(value));
        }
        public static void WriteDouble(this BinaryWriter writer, double value)
        {
            writer.Write(BitConverter.GetBytes(value));
        }

        #endregion
    }
}

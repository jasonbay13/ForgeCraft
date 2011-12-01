using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace SMP
{
    /// <summary>
    /// Various utilities for Minecraft.
    /// </summary>
    public static class MCUtil
    {
        /// <summary>
        /// Utilities for working with certain data types in the Minecraft protocol.
        /// </summary>
        public static class Protocol
        {
            /// <summary>
            /// Gets an array of bytes for a string.
            /// </summary>
            /// <param name="source">The string to get the bytes of.</param>
            /// <returns>Byte array of string.</returns>
            public static byte[] GetBytes(string source)
            {
                if (source.Length > short.MaxValue) throw new ArgumentException("String too big.");

                byte[] bytes = new byte[2 + (source.Length*2)];
                util.EndianBitConverter.Big.GetBytes((short) source.Length).CopyTo(bytes, 0);
                Encoding.BigEndianUnicode.GetBytes(source).CopyTo(bytes, 2);
                return bytes;
            }

            /// <summary>
            /// Gets the length of an array of bytes for a string.
            /// </summary>
            /// <param name="source">The string to get the bytes of.</param>
            /// <returns>Byte array of string.</returns>
            public static short GetBytesLength(string source)
            {
                if (source.Length > short.MaxValue) throw new ArgumentException("String too big.");
                return (short) (2 + source.Length*2);
            }

            /// <summary>
            /// Gets a string from a byte array starting at the specified index
            /// </summary>
            /// <param name="bytes">Byte array to get string from.</param>
            /// <param name="index">Index to starting read bytes at.</param>
            /// <returns>String read from the byte array.</returns>
            public static string GetString(byte[] bytes, int index)
            {
                return GetString(bytes, index, short.MaxValue);
            }

            /// <summary>
            /// Gets a string from a byte array starting at the specified index
            /// </summary>
            /// <param name="bytes">Byte array to get string from.</param>
            /// <param name="index">Index to starting read bytes at.</param>
            /// <param name="maxLength">Maximum length of the string to read.</param>
            /// <returns>String read from the byte array.</returns>
            public static string GetString(byte[] bytes, int index, short maxLength)
            {
                short length = util.EndianBitConverter.Big.ToInt16(bytes, index);
                if (length > maxLength)
                    throw new Exception(
                        new StringBuilder("Recieved tring length is longer than maximum allowed. (").Append(length).
                            Append(" > ").Append(maxLength).Append(")").ToString());
                if (length < 0) throw new Exception("Received string length is less than zero! Weird string!");

                return Encoding.BigEndianUnicode.GetString(bytes, index + 2, length*2);
            }

            /// <summary>
            /// Gets the length of a string from a byte array starting at the specified index
            /// </summary>
            /// <param name="bytes">Byte array to get string from.</param>
            /// <param name="index">Index to starting read bytes at.</param>
            /// <returns>String read from the byte array.</returns>
            public static short GetStringLength(byte[] bytes, int index)
            {
                return GetStringLength(bytes, index, short.MaxValue);
            }

            /// <summary>
            /// Gets the length of a string from a byte array starting at the specified index
            /// </summary>
            /// <param name="bytes">Byte array to get string from.</param>
            /// <param name="index">Index to starting read bytes at.</param>
            /// <param name="maxLength">Maximum length of the string to read.</param>
            /// <returns>String read from the byte array.</returns>
            public static short GetStringLength(byte[] bytes, int index, short maxLength)
            {
                short length = util.EndianBitConverter.Big.ToInt16(bytes, index);
                if (length > maxLength)
                    throw new Exception(
                        new StringBuilder("Recieved tring length is longer than maximum allowed. (").Append(length).
                            Append(" > ").Append(maxLength).Append(")").ToString());
                if (length < 0) throw new Exception("Received string length is less than zero! Weird string!");

                return (short) (2 + length*2);
            }
        }

        public static class Entities
        {
            public static byte[] GetMetaBytes(object[] data)
            {
                List<byte> bytes = new List<byte>(); object obj;
                for (int i = 0; i < data.Length && i < 32; i++) // Maximum index is 31 due to the index being 5 bits.
                {
                    obj = data[i];
                    if (obj == null) continue;
                    if (obj.GetType() == typeof(byte))
                    {
                        bytes.Add((byte)(i & 0x1F));
                        bytes.Add((byte)obj);
                    }
                    else if (obj.GetType() == typeof(short))
                    {
                        bytes.Add((byte)(0x01 << 5 | i & 0x1F));
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((short)obj));
                    }
                    else if (obj.GetType() == typeof(int))
                    {
                        bytes.Add((byte)(0x02 << 5 | i & 0x1F));
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((int)obj));
                    }
                    else if (obj.GetType() == typeof(float))
                    {
                        bytes.Add((byte)(0x03 << 5 | i & 0x1F));
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((float)obj));
                    }
                    else if (obj.GetType() == typeof(string))
                    {
                        bytes.Add((byte)(0x04 << 5 | i & 0x1F));
                        bytes.AddRange(MCUtil.Protocol.GetBytes((string)obj));
                    }
                    else if (obj.GetType() == typeof(Item))
                    {
                        Item item = (Item)obj;
                        bytes.Add((byte)(0x05 << 5 | i & 0x1F));
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes(item.id));
                        bytes.Add(item.count);
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes(item.meta));
                    }
                    else if (obj.GetType() == typeof(Point3))
                    {
                        Point3 point = (Point3)obj;
                        bytes.Add((byte)(0x06 << 5 | i & 0x1F));
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((int)point.x));
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((int)point.y));
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((int)point.z));
                    }
                }
                bytes.Add(0x7F);
                return bytes.ToArray();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

                byte[] bytes = new byte[2 + (source.Length * 2)];
                util.EndianBitConverter.Big.GetBytes((short)source.Length).CopyTo(bytes, 0);
                Encoding.BigEndianUnicode.GetBytes(source).CopyTo(bytes, 2);
                return bytes;
            }

            /// <summary>
            /// Gets the length of an array of bytes for a string.
            /// </summary>
            /// <param name="source">The string to get the bytes of.</param>
            /// <returns>Byte array of string.</returns>
            public static int GetBytesLength(string source)
            {
                if (source.Length > short.MaxValue) throw new ArgumentException("String too big.");
                return 2 + (source.Length * 2);
            }

            /// <summary>
            /// Gets a string from a byte array starting at the specified index
            /// </summary>
            /// <param name="bytes">Byte array to get string from.</param>
            /// <param name="index">Index to starting read bytes at.</param>
            /// <returns>String read from the byte array.</returns>
            public static string GetString(byte[] bytes, int index, int maxLength)
            {
                short length = util.EndianBitConverter.Big.ToInt16(bytes, index);
                if (length > maxLength) throw new Exception(new StringBuilder("Recieved tring length is longer than maximum allowed. (").Append(length).Append(" > ").Append(maxLength).Append(")").ToString());
                if (length < 0) throw new Exception("Received string length is less than zero! Weird string!");

                return Encoding.BigEndianUnicode.GetString(bytes, index + 2, util.EndianBitConverter.Big.ToInt16(bytes, index));
            }

            /// <summary>
            /// Gets the length of a string from a byte array starting at the specified index
            /// </summary>
            /// <param name="bytes">Byte array to get string from.</param>
            /// <param name="index">Index to starting read bytes at.</param>
            /// <returns>String read from the byte array.</returns>
            public static int GetStringLength(byte[] bytes, int index, int maxLength)
            {
                short length = util.EndianBitConverter.Big.ToInt16(bytes, index);
                if (length > maxLength) throw new Exception(new StringBuilder("Recieved tring length is longer than maximum allowed. (").Append(length).Append(" > ").Append(maxLength).Append(")").ToString());
                if (length < 0) throw new Exception("Received string length is less than zero! Weird string!");

                return length;
            }
        }
    }
}

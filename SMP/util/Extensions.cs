/*
	Copyright 2011 ForgeCraft team
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using System.IO.Compression;
using Ionic.Zlib;

namespace SMP
{
    public static class Extensions
    {
        /// <summary>
        /// Truncates a string to the specified maximum length.
        /// </summary>
        /// <param name="source">The string to be truncated.</param>
        /// <param name="maxLength">The maximum length to truncate the string to.</param>
        /// <returns>The truncated string.</returns>
        public static string Truncate(this string source, int maxLength)
        {
            if (source.Length > maxLength)
                source = source.Substring(0, maxLength);
            return source;
        }

        /// <summary>
        /// Truncates an array to the specified maximum length.
        /// </summary>
        /// <param name="source">The array to be truncated.</param>
        /// <param name="maxLength">The maximum length to truncate the array to.</param>
        /// <returns>The truncated array.</returns>
        public static Array Truncate(this Array source, int maxLength)
        {
            if (source.Length > maxLength)
                Array.Copy(source, 0, source, 0, maxLength);
            return source;
        }

        [Obsolete("Learn to use bit operators!", false)]
        public static byte GetBits(this byte data, int index, int count)
        {
            int max = (int)Math.Pow(2, count) - 1;
            return (byte)((data & (max << index)) >> index);
        }

        [Obsolete("Learn to use bit operators!", false)]
        public static byte SetBits(this byte data, int index, int value)
        {
            return (byte)(data | (value << index));
        }

        /// <summary>
        /// Compresses a byte array using Zlib.
        /// </summary>
        /// <param name="bytes">The byte array to be compressed.</param>
        /// <returns>Compressed byte array.</returns>
        public static byte[] Compress(this byte[] bytes)
        {
            return bytes.Compress(CompressionLevel.Default);
        }

        /// <summary>
        /// Compresses a byte array using Zlib.
        /// </summary>
        /// <param name="bytes">The byte array to be compressed.</param>
        /// <param name="level">Amount of compression to use.</param>
        /// <returns>Compressed byte array.</returns>
        public static byte[] Compress(this byte[] bytes, CompressionLevel level)
        {
            return bytes.Compress(level, CompressionType.Zlib);
        }

        /// <summary>
        /// Compresses a byte array using the specified compression type.
        /// </summary>
        /// <param name="bytes">The byte array to be compressed.</param>
        /// <param name="type">Type of compression to use.</param>
        /// <returns>Compressed byte array.</returns>
        public static byte[] Compress(this byte[] bytes, CompressionType type)
        {
            return bytes.Compress(CompressionLevel.Default, type);
        }

        /// <summary>
        /// Compresses a byte array using the specified compression type.
        /// </summary>
        /// <param name="bytes">The byte array to be compressed.</param>
        /// <param name="level">Amount of compression to use.</param>
        /// <param name="type">Type of compression to use.</param>
        /// <returns>Compressed byte array.</returns>
        public static byte[] Compress(this byte[] bytes, CompressionLevel level, CompressionType type)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                switch (type) {
                    case CompressionType.Zlib:
                        using (ZlibStream stream = new ZlibStream(memory, CompressionMode.Compress, level, true))
                            stream.Write(bytes, 0, bytes.Length);
                        break;
                    case CompressionType.GZip:
                        using (GZipStream stream = new GZipStream(memory, CompressionMode.Compress, level, true))
                            stream.Write(bytes, 0, bytes.Length);
                        break;
                    default:
                        throw new ArgumentException("Unknown compression type.");
                }
                memory.Position = 0;
                bytes = new byte[memory.Length];
                memory.Read(bytes, 0, (int)memory.Length);
            }
            return bytes;
        }

        /// <summary>
        /// Decompresses a byte array using Zlib.
        /// </summary>
        /// <param name="bytes">The byte array to be decompressed.</param>
        /// <returns>Decompressed byte array.</returns>
        public static byte[] Decompress(this byte[] bytes)
        {
            return bytes.Decompress(CompressionType.Zlib);
        }

        /// <summary>
        /// Decompresses a byte array using the specified compression type.
        /// </summary>
        /// <param name="bytes">The byte array to be decompressed.</param>
        /// <param name="type">Type of compression to use.</param>
        /// <returns>Decompressed byte array.</returns>
        public static byte[] Decompress(this byte[] bytes, CompressionType type)
        {
            int size = 4096;
            byte[] buffer = new byte[size];
            using (MemoryStream memory = new MemoryStream())
            {
                using (MemoryStream memory2 = new MemoryStream(bytes))
                    switch (type)
                    {
                        case CompressionType.Zlib:
                            using (ZlibStream stream = new ZlibStream(memory2, CompressionMode.Decompress))
                            {
                                int count = 0;
                                while ((count = stream.Read(buffer, 0, size)) > 0)
                                    memory.Write(buffer, 0, count);
                            }
                            break;
                        case CompressionType.GZip:
                            using (GZipStream stream = new GZipStream(memory2, CompressionMode.Decompress))
                            {
                                int count = 0;
                                while ((count = stream.Read(buffer, 0, size)) > 0)
                                    memory.Write(buffer, 0, count);
                            }
                            break;
                        default:
                            throw new ArgumentException("Unknown compression type.");
                    }
                return memory.ToArray();
            }
        }

        /// <summary>
        /// Capitalizes a string, duh!
        /// </summary>
        /// <param name="str">String to be capitalized.</param>
        /// <returns>The capitalized string.</returns>
        public static string Capitalize(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return String.Empty;
            char[] a = str.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }

    public enum CompressionType
    {
        Zlib,
        GZip
    }
}

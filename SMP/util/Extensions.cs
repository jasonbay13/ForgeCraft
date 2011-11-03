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
        public static string Truncate(this string source, int maxLength)
        {
            if (source.Length > maxLength)
                source = source.Substring(0, maxLength);
            return source;
        }
        public static Array Truncate(this Array source, int maxLength)
        {
            if (source.Length > maxLength)
                Array.Copy(source, 0, source, 0, maxLength);
            return source;
        }

        public static byte GetBits(this byte data, int index, int count)
        {
            int max = (int)Math.Pow(2, count) - 1;
            return (byte)((data & (max << index)) >> index);
        }
        public static byte SetBits(this byte data, int index, int value)
        {
            return (byte)(data | (value << index));
        }

        /// <summary>
        /// Compresses a byte array using zlib.
        /// </summary>
        /// <param name="bytes">The byte array to be compressed.</param>
        /// <returns>Compressed byte array.</returns>
        public static byte[] Compress(this byte[] bytes)
        {
            return bytes.Compress(CompressionLevel.Default);
        }

        /// <summary>
        /// Compresses a byte array using zlib.
        /// </summary>
        /// <param name="bytes">The byte array to be compressed.</param>
        /// <param name="level">Amount of compression to use.</param>
        /// <returns>Compressed byte array.</returns>
        public static byte[] Compress(this byte[] bytes, CompressionLevel level)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (ZlibStream stream = new ZlibStream(memory, CompressionMode.Compress, level, true))
                    stream.Write(bytes, 0, bytes.Length);
                memory.Position = 0;
                bytes = new byte[memory.Length];
                memory.Read(bytes, 0, (int)memory.Length);
            }
            return bytes;
        }

        /// <summary>
        /// Decompresses a byte array using zlib.
        /// </summary>
        /// <param name="bytes">The byte array to be decompressed.</param>
        /// <returns>Decompressed byte array.</returns>
        public static byte[] Decompress(this byte[] bytes)
        {
            int size = 4096;
            byte[] buffer = new byte[size];
            using (MemoryStream memory = new MemoryStream())
            {
                using (ZlibStream stream = new ZlibStream(new MemoryStream(bytes), CompressionMode.Decompress))
                {
                    int count = 0;
                    while ((count = stream.Read(buffer, 0, size)) > 0)
                        memory.Write(buffer, 0, count);
                }
                return memory.ToArray();
            }
        }
    }
}

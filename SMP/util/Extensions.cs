using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

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

        public static byte[] Compress(this byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gs = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    gs.Write(bytes, 0, bytes.Length);
                }
                ms.Position = 0;
                bytes = new byte[ms.Length];
                ms.Read(bytes, 0, (int)ms.Length);
            }
            return bytes;
        }
        public static byte[] Decompress(this byte[] bytes)
        {
            int size = 4096;
            byte[] buffer = new byte[size];
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream stream = new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress))
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

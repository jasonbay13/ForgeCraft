using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}

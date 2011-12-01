/*
	Copyright 2011 MCForge
	
	Author: fenderrock87
	
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

namespace SMP
{
    public static class MathHelper
    {
        private static float[] SIN_TABLE = InitSinTable();

        private static float[] InitSinTable()
        {
            float[] SIN_TABLE = new float[0x10000];
            for(int i = 0; i < 0x10000; i++)
            {
                SIN_TABLE[i] = (float)java.lang.Math.sin(((double)i * 3.1415926535897931D * 2D) / 65536D);
            }
            return SIN_TABLE;
        }

        #region Clamp()
        public static byte Clamp(byte value, byte low, byte high)
        {
            return Math.Max(Math.Min(value, high), low);
        }

        public static sbyte Clamp(sbyte value, sbyte low, sbyte high)
        {
            return Math.Max(Math.Min(value, high), low);
        }

        public static short Clamp(short value, short low, short high)
        {
            return Math.Max(Math.Min(value, high), low);
        }

        public static ushort Clamp(ushort value, ushort low, ushort high)
        {
            return Math.Max(Math.Min(value, high), low);
        }

        public static int Clamp(int value, int low, int high)
        {
            return Math.Max(Math.Min(value, high), low);
        }

        public static uint Clamp(uint value, uint low, uint high)
        {
            return Math.Max(Math.Min(value, high), low);
        }

        public static long Clamp(long value, long low, long high)
        {
            return Math.Max(Math.Min(value, high), low);
        }

        public static ulong Clamp(ulong value, ulong low, ulong high)
        {
            return Math.Max(Math.Min(value, high), low);
        }

        public static float Clamp(float value, float low, float high)
        {
            return Math.Max(Math.Min(value, high), low);
        }

        public static double Clamp(double value, double low, double high)
        {
            return Math.Max(Math.Min(value, high), low);
        }

        public static decimal Clamp(decimal value, decimal low, decimal high)
        {
            return Math.Max(Math.Min(value, high), low);
        }
        #endregion

        public static int floor_double(double d)
        {
            int i = (int)d;
            return d >= (double)i ? i : i - 1;
        }

        public static long func_35477_c(double d)
        {
            long l = (long)d;
            return d >= (double)l ? l : l - 1L;
        }

        public static float sin(float f)
        {
            return SIN_TABLE[(int)(f * 10430.38F) & 0xffff];
        }

        public static float sqrt_float(float f)
        {
            return (float)java.lang.Math.sqrt(f);
        }

        public static float sqrt_double(double d)
        {
            return (float)java.lang.Math.sqrt(d);
        }

        public static float cos(float f)
        {
            return SIN_TABLE[(int)(f * 10430.38F + 16384F) & 0xffff];
        }

        public static float abs(float f)
        {
            return f < 0.0F ? -f : f;
        }

        public static long RandomLong(Random random)
        {
            return (long)((random.NextDouble() * 2.0 - 1.0) * long.MaxValue);
        }
    }
}

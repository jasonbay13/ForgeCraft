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
        private static float[] SIN_TABLE;

        public static void Init()
        {
            SIN_TABLE = new float[0x10000];
            for(int i = 0; i < 0x10000; i++)
            {
                SIN_TABLE[i] = (float)java.lang.Math.sin(((double)i * 3.1415926535897931D * 2D) / 65536D);
            }
        }


        public static decimal Clamp(decimal value, decimal low, decimal high)
        {
            return Math.Max(Math.Min(value, high), low);
        }

        public static double Clamp(double value, double low, double high)
        {
            return Math.Max(Math.Min(value, high), low);
        }

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

        public static long pow(long x, long n)
        {
            if (n == 0) return 1;
            if (n == 1) return x;

            return x;
        }

        public static long RandomLong(Random random)
        {
            return (long)((random.NextDouble() * 2.0 - 1.0) * long.MaxValue);
        }
    }
}

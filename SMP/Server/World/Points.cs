using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public struct Point : IEquatable<Point>
    {
        public int x
        {
            get { return X; }
            set { X = value; }
        }
        public int z
        {
            get { return Z; }
            set { Z = value; }
        }
        public int X;
        public int Z;

        public Point(int X, int Y)
        {
            this.X = X;
            this.Z = Y;
        }

        public static bool operator ==(Point a, Point b)
        {
            if (a.x == b.x && a.z == b.z) return true;
            return false;
        }
        public static bool operator !=(Point a, Point b)
        {
            if (a.x != b.x || a.z != b.z) return true;
            return false;
        }
        public static Point operator *(Point a, int b)
        {
            try
            {
                a.x = (int)(a.x * b);
                a.z = (int)(a.z * b);
                return a;
            }
            catch
            {
                return Zero;
            }
        }
        public static Point operator /(Point a, int b)
        {
            try
            {
                a.x = (int)(a.x / b);
                a.z = (int)(a.z / b);
                return a;
            }
            catch
            {
                return Zero;
            }
        }

        public static Point Zero
        {
            get
            {
                return new Point(0, 0);
            }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public bool Equals(Point other)
        {
            if (this == other) return true;
            return false;
        }
        public override string ToString()
        {
            return base.ToString();
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public struct Point3 : IEquatable<Point3>
    {
        public double x
        {
            get { return X; }
            set { X = value; }
        }
        public double y
        {
            get { return Y; }
            set { Y = value; }
        }
        public double z
        {
            get { return Z; }
            set { Z = value; }
        }
        public double X;
        public double Y;
        public double Z;

        public Point3(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
        public Point3(double d)
        {
            X = d;
            Y = d;
            Z = d;
        }
        public Point3(int[] iar)
        {
            X = iar[0];
            Y = iar[1];
            Z = iar[2];
        }
        public Point3(double[] iar)
        {
            X = iar[0];
            Y = iar[1];
            Z = iar[2];
        }

        public static bool operator ==(Point3 a, Point3 b)
        {
            if (a.X == b.X && a.Y == b.Y && a.Z == b.Z) return true;
            return false;
        }
        public static bool operator ==(Point3 a, int[] b)
        {
            if (RD(a.X) == b[0] && RD(a.Y) == b[1] && RD(a.Z) == b[2]) return true;
            return false;
        }
        public static bool operator ==(Point3 a, double[] b)
        {
            if (a.X == b[0] && a.Y == b[1] && a.Z == b[2]) return true;
            return false;
        }

        public static bool operator !=(Point3 a, Point3 b)
        {
            if (a.x != b.x || a.y != b.y || a.z != b.z) return true;
            return false;
        }
        public static bool operator !=(Point3 a, int[] b)
        {
            if (RD(a.x) != b[0] || RD(a.y) != b[1] || RD(a.z) != b[2]) return true;
            return false;
        }
        public static bool operator !=(Point3 a, double[] b)
        {
            if (a.x != b[0] || a.y != b[1] || a.z != b[2]) return true;
            return false;
        }

        public static Point3 operator *(Point3 a, int b)
        {
            try
            {
                a.x = (int)(a.x * b);
                a.y = (int)(a.y * b);
                a.z = (int)(a.z * b);
                return a;
            }
            catch
            {
                return Zero;
            }
        }
        public static Point3 operator *(Point3 a, Point3 b)
        {
            try
            {
                a.x = a.x * b.x;
                a.y = a.y * b.y;
                a.z = a.z * b.z;
                return a;
            }
            catch
            {
                return Zero;
            }
        }

        public static Point3 operator /(Point3 a, int b)
        {
            try
            {
                a.x = (int)(a.x / b);
                a.y = (int)(a.y / b);
                a.z = (int)(a.z / b);
                return a;
            }
            catch
            {
                return Zero;
            }
        }
        public static Point3 operator /(Point3 a, Point3 b)
        {
            try
            {
                a.x = a.x / b.x;
                a.y = a.y / b.y;
                a.z = a.z / b.z;
                return a;
            }
            catch
            {
                return Zero;
            }
        }

        public static Point3 operator -(Point3 a, int b)
        {
            a.x = a.x - b;
            a.y = a.y - b;
            a.z = a.z - b;
            return a;
        }
        public static Point3 operator -(Point3 a, Point3 b)
        {
            a.x = a.x - b.x;
            a.y = a.y - b.y;
            a.z = a.z - b.z;
            return a;
        }

        public static Point3 operator +(Point3 a, int b)
        {
            a.x = a.x + b;
            a.y = a.y + b;
            a.z = a.z + b;
            return a;
        }
        public static Point3 operator +(Point3 a, Point3 b)
        {
            a.x = a.x + b.x;
            a.y = a.y + b.y;
            a.z = a.z + b.z;
            return a;
        }

        public Point3 diff(Point3 a)
        {
            a.x = Math.Abs(Math.Max(a.X, X) - Math.Min(a.X, X));
            a.y = Math.Abs(Math.Max(a.Y, Y) - Math.Min(a.Y, Y));
            a.z = Math.Abs(Math.Max(a.Z, Z) - Math.Min(a.Z, Z));
            return a;
        }
        public double mdiff(Point3 a)
        {
            a.x = Math.Abs(a.X - X);
            a.y = Math.Abs(a.Y - Y);
            a.z = Math.Abs(a.Z - Z);
            return Math.Max(Math.Max(a.x, a.y), a.z);
        }

        public double distance(Point3 a)
        {
            return Math.Sqrt(Math.Pow(a.x - x, 2) + Math.Pow(a.y - x, 2) + Math.Pow(a.z - z, 2));
        }

        static public implicit operator Point3(int[] value)
        {
            return new Point3(value);
        }
        static public implicit operator Point3(double[] value)
        {
            return new Point3(value);
        }
        static public explicit operator int[](Point3 po)
        {
            return new int[3] { (int)RD(po.x), (int)RD(po.y), (int)RD(po.z) };
        }
        static public explicit operator double[](Point3 po)
        {
            return new double[3] { po.x, po.y, po.z };
        }

        public static Point3 Zero { get { return new Point3(0, 0, 0); } }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public bool Equals(Point3 other)
        {
            if (this == other) return true;
            return false;
        }
        public override string ToString()
        {
            return base.ToString();
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Point3 RD()
        {
            return new Point3(RD(x), RD(y), RD(z));
        }
        static double RD(double valueToRound)
        {
            /*if (valueToRound < 0) // Can you say... redundancy?
            {
                return Math.Floor(valueToRound);
            }
            else
            {
                return Math.Floor(valueToRound);
            }*/

            return Math.Floor(valueToRound);
        }
    }
}

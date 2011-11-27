using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP.Generator
{
    public class StructureBoundingBox
    {
        public int x1;
        public int y1;
        public int z1;
        public int x2;
        public int y2;
        public int z2;

        public StructureBoundingBox()
        {
        }

        public static StructureBoundingBox func_35672_a()
        {
            unchecked { return new StructureBoundingBox(0x7fffffff, 0x7fffffff, 0x7fffffff, (int)0x80000000, (int)0x80000000, (int)0x80000000); }
        }

        public static StructureBoundingBox func_35663_a(int i, int j, int k, int l, int i1, int j1, int k1, int l1, int i2, int j2)
        {
            switch (j2)
            {
                default:
                    return new StructureBoundingBox(i + l, j + i1, k + j1, ((i + k1) - 1) + l, ((j + l1) - 1) + i1, ((k + i2) - 1) + j1);

                case 2: // '\002'
                    return new StructureBoundingBox(i + l, j + i1, (k - i2) + 1 + j1, ((i + k1) - 1) + l, ((j + l1) - 1) + i1, k + j1);

                case 0: // '\0'
                    return new StructureBoundingBox(i + l, j + i1, k + j1, ((i + k1) - 1) + l, ((j + l1) - 1) + i1, ((k + i2) - 1) + j1);

                case 1: // '\001'
                    return new StructureBoundingBox((i - i2) + 1 + j1, j + i1, k + l, i + j1, ((j + l1) - 1) + i1, ((k + k1) - 1) + l);

                case 3: // '\003'
                    return new StructureBoundingBox(i + j1, j + i1, k + l, ((i + i2) - 1) + j1, ((j + l1) - 1) + i1, ((k + k1) - 1) + l);
            }
        }

        public StructureBoundingBox(StructureBoundingBox structureboundingbox)
        {
            x1 = structureboundingbox.x1;
            y1 = structureboundingbox.y1;
            z1 = structureboundingbox.z1;
            x2 = structureboundingbox.x2;
            y2 = structureboundingbox.y2;
            z2 = structureboundingbox.z2;
        }

        public StructureBoundingBox(int i, int j, int k, int l, int i1, int j1)
        {
            x1 = i;
            y1 = j;
            z1 = k;
            x2 = l;
            y2 = i1;
            z2 = j1;
        }

        public StructureBoundingBox(int i, int j, int k, int l)
        {
            x1 = i;
            y1 = 0;
            z1 = j;
            x2 = k;
            y2 = 0x10000;
            z2 = l;
        }

        public bool canFitInside(StructureBoundingBox structureboundingbox)
        {
            return x2 >= structureboundingbox.x1 && x1 <= structureboundingbox.x2 && z2 >= structureboundingbox.z1 && z1 <= structureboundingbox.z2 && y2 >= structureboundingbox.y1 && y1 <= structureboundingbox.y2;
        }

        public bool isInBbArea(int i, int j, int k, int l)
        {
            return x2 >= i && x1 <= k && z2 >= j && z1 <= l;
        }

        public void expandTo(StructureBoundingBox structureboundingbox)
        {
            x1 = Math.Min(x1, structureboundingbox.x1);
            y1 = Math.Min(y1, structureboundingbox.y1);
            z1 = Math.Min(z1, structureboundingbox.z1);
            x2 = Math.Max(x2, structureboundingbox.x2);
            y2 = Math.Max(y2, structureboundingbox.y2);
            z2 = Math.Max(z2, structureboundingbox.z2);
        }

        public void offset(int i, int j, int k)
        {
            x1 += i;
            y1 += j;
            z1 += k;
            x2 += i;
            y2 += j;
            z2 += k;
        }

        public bool isInBbVolume(int i, int j, int k)
        {
            return i >= x1 && i <= x2 && k >= z1 && k <= z2 && j >= y1 && j <= y2;
        }

        public int bbWidth()
        {
            return (x2 - x1) + 1;
        }

        public int bbHeight()
        {
            return (y2 - y1) + 1;
        }

        public int bbDepth()
        {
            return (z2 - z1) + 1;
        }

        public int func_40623_e()
        {
            return x1 + ((x2 - x1) + 1) / 2;
        }

        public int func_40622_f()
        {
            return y1 + ((y2 - y1) + 1) / 2;
        }

        public int func_40624_g()
        {
            return z1 + ((z2 - z1) + 1) / 2;
        }

        public override string ToString()
        {
            return (new StringBuilder()).Append("(").Append(x1).Append(", ").Append(y1).Append(", ").Append(z1).Append("; ").Append(x2).Append(", ").Append(y2).Append(", ").Append(z2).Append(")").ToString();
        }
    }
}

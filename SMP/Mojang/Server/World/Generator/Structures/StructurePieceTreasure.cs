using System;

namespace SMP.Generator
{
    public class StructurePieceTreasure : WeightedRandomChoice
    {
        public int field_35489_a;
        public int field_35487_b;
        public int field_35488_c;
        public int field_35486_e;

        public StructurePieceTreasure(int i, int j, int k, int l, int i1)
            : base(i1)
        {
            field_35489_a = i;
            field_35487_b = j;
            field_35488_c = k;
            field_35486_e = l;
        }
    }
}

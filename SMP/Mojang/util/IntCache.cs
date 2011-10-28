using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public class IntCache
    {
        public IntCache()
        {
        }

        public static int[] func_35549_a(int i)
        {
            if(i <= 256)
            {
                if(field_35553_b.Count == 0)
                {
                    int[] ai = new int[256];
                    field_35554_c.Add(ai);
                    return ai;
                } else
                {
                    int[] ai1 = (int[])field_35553_b[field_35553_b.Count - 1];
                    field_35553_b.RemoveAt(field_35553_b.Count - 1);
                    field_35554_c.Add(ai1);
                    return ai1;
                }
            }
            if(i > field_35555_a)
            {
                //Console.WriteLine((new StringBuilder()).Append("New max size: ").Append(i).ToString());
                field_35555_a = i;
                field_35551_d.Clear();
                field_35552_e.Clear();
                int[] ai2 = new int[field_35555_a];
                field_35552_e.Add(ai2);
                return ai2;
            }
            if(field_35551_d.Count == 0)
            {
                int[] ai3 = new int[field_35555_a];
                field_35552_e.Add(ai3);
                return ai3;
            } else
            {
                int[] ai4 = (int[])field_35551_d[field_35551_d.Count - 1];
                field_35551_d.RemoveAt(field_35551_d.Count - 1);
                field_35552_e.Add(ai4);
                return ai4;
            }
        }

        public static void func_35550_a()
        {
            if (field_35551_d.Count > 0)
            {
                field_35551_d.RemoveAt(field_35551_d.Count - 1);
            }
            if (field_35553_b.Count > 0)
            {
                field_35553_b.RemoveAt(field_35553_b.Count - 1);
            }
            field_35551_d.AddRange(field_35552_e);
            field_35553_b.AddRange(field_35554_c);
            field_35552_e.Clear();
            field_35554_c.Clear();
        }

        private static int field_35555_a = 256;
        private static List<int[]> field_35553_b = new List<int[]>();
        private static List<int[]> field_35554_c = new List<int[]>();
        private static List<int[]> field_35551_d = new List<int[]>();
        private static List<int[]> field_35552_e = new List<int[]>();
    }
}

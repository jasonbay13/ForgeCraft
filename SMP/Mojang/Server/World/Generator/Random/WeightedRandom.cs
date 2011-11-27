using System;
using System.Collections.Generic;

namespace SMP.Generator
{
    public class WeightedRandom
    {
        public WeightedRandom()
        {
        }

        public static int func_35692_a(ICollection<WeightedRandomChoice> collection)
        {
            int i = 0;
            for (IEnumerator<WeightedRandomChoice> iterator = collection.GetEnumerator(); iterator.MoveNext(); )
            {
                WeightedRandomChoice weightedrandomchoice = (WeightedRandomChoice)iterator.Current;
                i += weightedrandomchoice.field_35483_d;
            }

            return i;
        }

        public static WeightedRandomChoice func_35693_a(java.util.Random random, ICollection<WeightedRandomChoice> collection, int i)
        {
            if(i <= 0)
            {
                throw new ArgumentException();
            }
            int j = random.nextInt(i);
            for(IEnumerator<WeightedRandomChoice> iterator = collection.GetEnumerator(); iterator.MoveNext();)
            {
                WeightedRandomChoice weightedrandomchoice = (WeightedRandomChoice)iterator.Current;
                j -= weightedrandomchoice.field_35483_d;
                if(j < 0)
                {
                    return weightedrandomchoice;
                }
            }

            return null;
        }

        public static WeightedRandomChoice func_35689_a(java.util.Random random, ICollection<WeightedRandomChoice> collection)
        {
            return func_35693_a(random, collection, func_35692_a(collection));
        }

        public static int func_35690_a(WeightedRandomChoice[] aweightedrandomchoice)
        {
            int i = 0;
            WeightedRandomChoice[] aweightedrandomchoice1 = aweightedrandomchoice;
            int j = aweightedrandomchoice1.Length;
            for(int k = 0; k < j; k++)
            {
                WeightedRandomChoice weightedrandomchoice = aweightedrandomchoice1[k];
                i += weightedrandomchoice.field_35483_d;
            }

            return i;
        }

        public static WeightedRandomChoice func_35688_a(java.util.Random random, WeightedRandomChoice[] aweightedrandomchoice, int i)
        {
            if(i <= 0)
            {
                throw new ArgumentException();
            }
            int j = random.nextInt(i);
            WeightedRandomChoice[] aweightedrandomchoice1 = aweightedrandomchoice;
            int k = aweightedrandomchoice1.Length;
            for(int l = 0; l < k; l++)
            {
                WeightedRandomChoice weightedrandomchoice = aweightedrandomchoice1[l];
                j -= weightedrandomchoice.field_35483_d;
                if(j < 0)
                {
                    return weightedrandomchoice;
                }
            }

            return null;
        }

        public static WeightedRandomChoice func_35691_a(java.util.Random random, WeightedRandomChoice[] aweightedrandomchoice)
        {
            return func_35688_a(random, aweightedrandomchoice, func_35690_a(aweightedrandomchoice));
        }
    }
}

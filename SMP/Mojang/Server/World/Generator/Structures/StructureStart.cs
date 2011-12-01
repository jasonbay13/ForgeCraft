using System;
using System.Collections.Generic;

namespace SMP.Generator
{
    public class StructureStart
    {
        protected LinkedList<StructureComponent> components;
        protected StructureBoundingBox boundingBox;

        protected StructureStart()
        {
            components = new LinkedList<StructureComponent>();
        }

        public StructureBoundingBox getBoundingBox()
        {
            return boundingBox;
        }

        public LinkedList<StructureComponent> func_40208_c()
        {
            return components;
        }

        public void generateStructure(World world, java.util.Random random, StructureBoundingBox structureboundingbox)
        {
            IEnumerator<StructureComponent> iterator = new LinkedList<StructureComponent>(components).GetEnumerator();
            do
            {
                if (!iterator.MoveNext())
                {
                    break;
                }
                StructureComponent structurecomponent = (StructureComponent)iterator.Current;
                if (structurecomponent.getStructureBoundingBox().canFitInside(structureboundingbox) && !structurecomponent.addComponentParts(world, random, structureboundingbox))
                {
                    components.Remove(iterator.Current);
                }
            } while (true);
        }

        protected void updateBoundingBox()
        {
            boundingBox = StructureBoundingBox.func_35672_a();
            StructureComponent structurecomponent;
            for (IEnumerator<StructureComponent> iterator = components.GetEnumerator(); iterator.MoveNext(); boundingBox.expandTo(structurecomponent.getStructureBoundingBox()))
            {
                structurecomponent = (StructureComponent)iterator.Current;
            }

        }

        protected void func_35545_a(World world, java.util.Random random, int i)
        {
            int j = world.worldOceanHeight - i;
            int k = boundingBox.bbHeight() + 1;
            if (k < j)
            {
                k += random.nextInt(j - k);
            }
            int l = k - boundingBox.y2;
            boundingBox.offset(0, l, 0);
            StructureComponent structurecomponent;
            for (IEnumerator<StructureComponent> iterator = components.GetEnumerator(); iterator.MoveNext(); structurecomponent.getStructureBoundingBox().offset(0, l, 0))
            {
                structurecomponent = (StructureComponent)iterator.Current;
            }

        }

        protected void func_40209_a(World world, java.util.Random random, int i, int j)
        {
            int k = ((j - i) + 1) - boundingBox.bbHeight();
            int l = 1;
            if (k > 1)
            {
                l = i + random.nextInt(k);
            }
            else
            {
                l = i;
            }
            int i1 = l - boundingBox.y1;
            boundingBox.offset(0, i1, 0);
            StructureComponent structurecomponent;
            for (IEnumerator<StructureComponent> iterator = components.GetEnumerator(); iterator.MoveNext(); structurecomponent.getStructureBoundingBox().offset(0, i1, 0))
            {
                structurecomponent = (StructureComponent)iterator.Current;
            }

        }

        public bool isSizeableStructure()
        {
            return true;
        }
    }
}

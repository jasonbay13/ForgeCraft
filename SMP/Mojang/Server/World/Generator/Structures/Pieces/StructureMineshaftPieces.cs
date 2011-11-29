using System;
using System.Collections.Generic;

namespace SMP.Generator
{
    public class StructureMineshaftPieces
    {
        private static readonly StructurePieceTreasure[] lootArray = InitLootArray();

        public StructureMineshaftPieces()
        {
        }

        private static StructureComponent getRandomComponent(List<StructureComponent> list, java.util.Random random, int i, int j, int k, int l, int i1)
        {
            int j1 = random.nextInt(100);
            /*if(j1 >= 80)
            {
                StructureBoundingBox structureboundingbox = ComponentMineshaftCross.func_35362_a(list, random, i, j, k, l);
                if(structureboundingbox != null)
                {
                    return new ComponentMineshaftCross(i1, random, structureboundingbox, l);
                }
            } else
            if(j1 >= 70)
            {
                StructureBoundingBox structureboundingbox1 = ComponentMineshaftStairs.func_35365_a(list, random, i, j, k, l);
                if(structureboundingbox1 != null)
                {
                    return new ComponentMineshaftStairs(i1, random, structureboundingbox1, l);
                }
            } else
            {
                StructureBoundingBox structureboundingbox2 = ComponentMineshaftCorridor.func_35357_a(list, random, i, j, k, l);
                if(structureboundingbox2 != null)
                {
                    return new ComponentMineshaftCorridor(i1, random, structureboundingbox2, l);
                }
            }*/
            return null;
        }

        private static StructureComponent getNextMineShaftComponent(StructureComponent structurecomponent, List<StructureComponent> list, java.util.Random random, int i, int j, int k, int l, int i1)
        {
            if(i1 > 8)
            {
                return null;
            }
            if(Math.Abs(i - structurecomponent.getStructureBoundingBox().x1) > 80 || Math.Abs(k - structurecomponent.getStructureBoundingBox().z1) > 80)
            {
                return null;
            }
            StructureComponent structurecomponent1 = getRandomComponent(list, random, i, j, k, l, i1 + 1);
            if(structurecomponent1 != null)
            {
                list.Add(structurecomponent1);
                structurecomponent1.buildComponent(structurecomponent, list, random);
            }
            return structurecomponent1;
        }

        static StructureComponent getNextComponent(StructureComponent structurecomponent, List<StructureComponent> list, java.util.Random random, int i, int j, int k, int l, int i1)
        {
            return getNextMineShaftComponent(structurecomponent, list, random, i, j, k, l, i1);
        }

        static StructurePieceTreasure[] getTreasurePieces()
        {
            return lootArray;
        }

        private static StructurePieceTreasure[] InitLootArray()
        {
            return (new StructurePieceTreasure[] {
                new StructurePieceTreasure((short)Items.IronIngot, 0, 1, 5, 10), new StructurePieceTreasure((short)Items.GoldIngot, 0, 1, 3, 5), new StructurePieceTreasure((short)Items.Redstone, 0, 4, 9, 5), new StructurePieceTreasure((short)Items.Dye, 4, 4, 9, 5), new StructurePieceTreasure((short)Items.Diamond, 0, 1, 2, 3), new StructurePieceTreasure((short)Items.Coal, 0, 3, 8, 10), new StructurePieceTreasure((short)Items.Bread, 0, 1, 3, 15), new StructurePieceTreasure((short)Items.IronPickaxe, 0, 1, 1, 1), new StructurePieceTreasure((byte)Blocks.Rails, 0, 4, 8, 1), new StructurePieceTreasure((short)Items.MelonSeeds, 0, 2, 4, 10), 
                new StructurePieceTreasure((short)Items.PumpkinSeeds, 0, 2, 4, 10)
            });
        }
    }
}

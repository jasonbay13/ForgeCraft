using System;

namespace SMP.Generator
{
    public abstract class StructurePieceBlockSelector
    {
        protected int selectedBlockId;
        protected int selectedBlockMetaData;

        protected StructurePieceBlockSelector()
        {
        }

        public abstract void selectBlocks(java.util.Random random, int i, int j, int k, bool flag);

        public int func_35566_a()
        {
            return selectedBlockId;
        }

        public int func_35567_b()
        {
            return selectedBlockMetaData;
        }
    }
}

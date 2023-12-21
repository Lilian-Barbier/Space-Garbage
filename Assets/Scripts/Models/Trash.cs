using System;
using Enums;

namespace Models
{
    public class Trash
    {
        public Block[][] Blocks { get; set; }
        public bool isAssembled { get; set; }

        public Trash() { }

        public Trash(Block[][] blocks)
        {
            Blocks = blocks;
        }

        public bool[][] GetBlocksAsBools()
        {
            var blocks = new bool[Blocks.Length][];
            for (var i = 0; i < Blocks.Length; i++)
            {
                blocks[i] = new bool[Blocks[i].Length];
                for (var j = 0; j < Blocks[i].Length; j++)
                {
                    blocks[i][j] = Blocks[i][j].Exists;
                }
            }
            return blocks;
        }

        public void SetColor(BlockColor color)
        {
            //for (var i = 0; i < Blocks.Length; i++)
            //{
            //    for (var j = 0; j < Blocks[i].Length; j++)
            //    {
            //        if (Blocks[i][j].Exists)
            //            Blocks[i][j].Color = color;
            //    }
            //}
        }

        public BlockColor GetColor()
        {
            //bool firstColorReach = false;
            //BlockColor color = BlockColor.None;
            //for (var i = 0; i < Blocks.Length; i++)
            //{
            //    for (var j = 0; j < Blocks[i].Length; j++)
            //    {
            //        if (Blocks[i][j].Exists)
            //        {
            //            if (!firstColorReach)
            //            {
            //                color = Blocks[i][j].Color;
            //            }
            //            else if(Blocks[i][j].Color != color)
            //            {
            //                return BlockColor.Failed;
            //            }
            //        }
            //    }
            //}
            //return color;
            return BlockColor.None;
        }

        public int GetTrashSize()
        {
            int size = 0;
            foreach (var line in Blocks)
            {
                foreach (var block in line)
                {
                    if (block.Exists)
                        size++;
                }
            }
            return size;
        }
    }
}


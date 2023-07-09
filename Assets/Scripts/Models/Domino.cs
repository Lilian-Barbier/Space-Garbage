using Enums;

namespace Models
{
    public class Domino
    {
        public Block[][] Blocks { get; set; }
        public bool isAssembled { get; set; }

        public Domino() { }

        public Domino(bool[][] blocks) : this(blocks, BlockColor.None) { }

        public Domino(bool[][] blocks, BlockColor color)
        {
            Blocks = new Block[4][];

            for (var i = 0; i < 4; i++)
            {
                Blocks[i] = new Block[4];
                for (var j = 0; j < 4; j++)
                {
                    Blocks[i][j] = new Block()
                    {
                        Exists = blocks[i][j],
                        Color = color
                    };
                }
            }
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
            for (var i = 0; i < Blocks.Length; i++)
            {
                for (var j = 0; j < Blocks[i].Length; j++)
                {
                    if (Blocks[i][j].Exists)
                        Blocks[i][j].Color = color;
                }
            }
        }

        public BlockColor GetColor()
        {
            bool firstColorReach = false;
            BlockColor color = BlockColor.None;
            for (var i = 0; i < Blocks.Length; i++)
            {
                for (var j = 0; j < Blocks[i].Length; j++)
                {
                    if (Blocks[i][j].Exists)
                    {
                        if (!firstColorReach)
                        {
                            color = Blocks[i][j].Color;
                        }
                        else if(Blocks[i][j].Color != color)
                        {
                            return BlockColor.Failed;
                        }
                    }
                }
            }
            return color;
        }

    }
}


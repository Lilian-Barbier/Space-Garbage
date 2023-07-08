using Enums;

namespace Models {
  public class Domino
  {
    public Block[][] Blocks { get; set; }

    public Domino() {}

    public Domino(bool[][] blocks)
    {
      Blocks = new Block[4][];

      for (var i = 0; i < 4; i++)
      {
        Blocks[i] = new Block[4];
        for (var j = 0; j < 4; j++)
        {
          Blocks[i][j] = new Block() {
            Exists = blocks[i][j],
            Color = BlockColor.Blue
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
  }
}


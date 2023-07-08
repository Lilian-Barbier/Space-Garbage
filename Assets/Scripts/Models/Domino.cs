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
      var blocks = new bool[4][];
      for (var i = 0; i < 4; i++)
      {
        blocks[i] = new bool[4];
        for (var j = 0; j < 4; j++)
        {
          blocks[i][j] = Blocks[i][j].Exists;
        }
      }
      return blocks;
    }
  }
}


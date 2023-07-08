using Enums;

namespace Models {
  public class Domino
  {
    public Block[][] Blocks { get; set; }

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


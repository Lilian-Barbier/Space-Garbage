using Enums;

namespace Models {
  public class DominoRequest
  {
    public TetrisPlayer Player { get; set; }    
    public bool[][] Blocks { get; set; }
    public BlockColor Color { get; set; }
  }
}


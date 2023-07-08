using Enums;

namespace Models {
  public class DominoRequest
  {
    public string PlayerName { get; set; }
    public int PlayerAge { get; set; }
    
    public bool[][] Blocks { get; set; }
    public BlockColor Color { get; set; }
  }
}


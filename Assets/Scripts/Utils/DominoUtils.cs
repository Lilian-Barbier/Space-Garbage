using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Enums;

namespace Utils {
  public class DominoUtils
  {
    #region Dominos

    public static readonly bool[][] Skew = new bool[][] {
      new bool[] {  true, false, false, false },
      new bool[] {  true,  true, false, false },
      new bool[] { false,  true, false, false },
      new bool[] { false, false, false, false }
    };

    public static readonly bool[][] ReverseSkew = new bool[][] {
      new bool[] { false,  true, false, false },
      new bool[] {  true,  true, false, false },
      new bool[] {  true, false, false, false },
      new bool[] { false, false, false, false }
    };

    public static readonly bool[][] Square = new bool[][] {
      new bool[] {  true,  true, false, false },
      new bool[] {  true,  true, false, false },
      new bool[] { false, false, false, false },
      new bool[] { false, false, false, false }
    };

    public static readonly bool[][] L = new bool[][] {
      new bool[] {  true, false, false, false },
      new bool[] {  true, false, false, false },
      new bool[] {  true,  true, false, false },
      new bool[] { false, false, false, false }
    };

    public static readonly bool[][] ReverseL = new bool[][] {
      new bool[] { false,  true, false, false },
      new bool[] { false,  true, false, false },
      new bool[] {  true,  true, false, false },
      new bool[] { false, false, false, false }
    };

    public static readonly bool[][] T = new bool[][] {
      new bool[] {  true, false, false, false },
      new bool[] {  true,  true, false, false },
      new bool[] {  true, false, false, false },
      new bool[] { false, false, false, false }
    };

    public static readonly bool[][] Line = new bool[][] {
      new bool[] {  true, false, false, false },
      new bool[] {  true, false, false, false },
      new bool[] {  true, false, false, false },
      new bool[] {  true, false, false, false }
    };

    public static readonly bool[][][] ValidDomino = new bool[][][] {
      Skew,
      ReverseSkew,
      Square,
      L,
      ReverseL,
      T,
      Line
    };

    public static readonly bool[][] SingleBlock = new bool[][] {
      new bool[] {  true, false, false, false },
      new bool[] { false, false, false, false },
      new bool[] { false, false, false, false },
      new bool[] { false, false, false, false }
    };

    public static readonly bool[][] DoubleBlock = new bool[][] {
      new bool[] {  true, false, false, false },
      new bool[] {  true, false, false, false },
      new bool[] { false, false, false, false },
      new bool[] { false, false, false, false }
    };

    public static readonly bool[][] TripleBlock = new bool[][] {
      new bool[] {  true, false, false, false },
      new bool[] {  true,  true, false, false },
      new bool[] { false, false, false, false },
      new bool[] { false, false, false, false }
    };

    #endregion

    public static bool[][] GetRandomValidDomino()
    {
      return ValidDomino[Random.Range(0, ValidDomino.Length)];
    }

    public static bool[][] GetMinimumDominoArea(bool[][] domino) {
      int minCol = 10000;
      int maxCol = 0;
      int minRow = 10000;
      int maxRow = 0;

      for (int row = 0; row < domino.Length; row ++) {
        for (int col = 0; col < domino[row].Length; col++) {
          if (domino[row][col]) {
            if (row < minRow) minRow = row;
            if (row > maxRow) maxRow = row;

            if (col < minCol) minCol = col;
            if (col > maxCol) maxCol = col;
          }
        }
      }

      bool[][] minimumDominoArea = new bool[maxRow - minRow + 1][];

      for(int x = 0; x < minimumDominoArea.Length; x++) {
        minimumDominoArea[x] = new bool[maxCol - minCol + 1];
        for(int y = 0; y < minimumDominoArea[x].Length; y++)
          minimumDominoArea[x][y] = domino[x + minRow][y + minCol];
      }      

      return minimumDominoArea;
    }

    public static Domino GetMinimumDominoArea(Domino domino) {
      int minCol = 10000;
      int maxCol = 0;
      int minRow = 10000;
      int maxRow = 0;

      for (int row = 0; row < domino.Blocks.Length; row ++) {
        for (int col = 0; col < domino.Blocks[row].Length; col++) {
          if (domino.Blocks[row][col].Exists) {
            if (row < minRow) minRow = row;
            if (row > maxRow) maxRow = row;

            if (col < minCol) minCol = col;
            if (col > maxCol) maxCol = col;
          }
        }
      }

      Block[][] minimumDominoArea = new Block[maxRow - minRow + 1][];

      for(int x = 0; x < minimumDominoArea.Length; x++) {
        minimumDominoArea[x] = new Block[maxCol - minCol + 1];
        for(int y = 0; y < minimumDominoArea[x].Length; y++)
          minimumDominoArea[x][y] = domino.Blocks[x + minRow][y + minCol];
      }

      return new Domino() {
        Blocks =  minimumDominoArea
      };
    }

    public static bool[][] RotateDominoClockwise(bool[][] domino) {
      bool[][] rotatedDomino = new bool[domino[0].Length][];

      for(int x = 0; x < rotatedDomino.Length; x++) {
        rotatedDomino[x] = new bool[domino.Length];
      }

      for(int x = 0; x < domino.Length; x++) {
        for(int y = 0; y < domino[x].Length; y++) {
          rotatedDomino[y][domino.Length - 1 - x] = domino[x][y];
        }
      }

      return rotatedDomino;
    }

    public static bool CompareDominos(bool[][] domino1, bool[][] domino2) {

      var rotatedDomino1 = RotateDominoClockwise(domino1);

      var minDomino1 = GetMinimumDominoArea(domino1);
      var minRotatedDomino1 = GetMinimumDominoArea(rotatedDomino1);
      var minDomino2 = GetMinimumDominoArea(domino2);

      if(minDomino1.Length == minDomino2.Length && minDomino1[0].Length == minDomino2[0].Length) {
        for(int x = 0; x < minDomino1.Length; x++) {
          for(int y = 0; y < minDomino1[x].Length; y++) {
            if(minDomino1[x][y] != minDomino2[x][y]) {
              return false;
            }
          }
        }

        return true;
      }

      if(minRotatedDomino1.Length == minDomino2.Length && minRotatedDomino1[0].Length == minDomino2[0].Length) {
        for(int x = 0; x < minRotatedDomino1.Length; x++) {
          for(int y = 0; y < minRotatedDomino1[x].Length; y++) {
            if(minRotatedDomino1[x][y] != minDomino2[x][y]) {
              return false;
            }
          }
        }

        return true;
      }

      return false;
    }

    public static bool isDominoFullfillingRequest(Domino domino, DominoRequest dominoRequest) 
    {  
      var isCorrectShape = CompareDominos(domino.GetBlocksAsBools(), dominoRequest.Blocks);

      if(!isCorrectShape) {
        return false;
      }

      var isCorrectColor = true;

      for(int x = 0; x < domino.Blocks.Length; x++) {
        for(int y = 0; y < domino.Blocks[x].Length; y++) {
          if(domino.Blocks[x][y].Exists && domino.Blocks[x][y].Color != dominoRequest.Color) {
            isCorrectColor = false;
          }
        }
      }

      return isCorrectShape && isCorrectColor;
    }

    public static BlockColor GetRandomColor()
    {
      var randomColor = Random.Range(1, BlockColor.GetNames(typeof(BlockColor)).Length);
      return (BlockColor) randomColor;
    }

    #region Debug

    public static string PrintDomino(bool[][] domino) {
      string dominoString = "";

      for(int x = 0; x < domino.Length; x++) {
        for(int y = 0; y < domino[x].Length; y++) {
          dominoString += domino[x][y] ? "■" : "□";
        }
        dominoString += "\n";
      }

      return dominoString;
    }

    #endregion

  }
}


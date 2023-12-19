using System.Collections.Generic;
using UnityEngine;
using Models;
using Enums;
using Assets.Scripts.Enums;

namespace Utils
{
    public class TrashUtils
    {
        #region Dominos

        public static readonly Block[][] Skew = new Block[][] {
        new Block[] { new Block(MaterialType.Organic),  new Block(),                    new Block(), new Block() },
        new Block[] { new Block(MaterialType.Organic),  new Block(MaterialType.Metal),  new Block(), new Block() },
        new Block[] { new Block(),                      new Block(MaterialType.Metal),  new Block(), new Block() },
        new Block[] { new Block(),                      new Block(),                    new Block(), new Block() }
      };

        public static readonly Block[][] ReverseSkew = new Block[][] {
        new Block[] { new Block(),                      new Block(MaterialType.Organic),    new Block(), new Block() },
        new Block[] { new Block(MaterialType.Organic),  new Block(MaterialType.Organic),    new Block(), new Block() },
        new Block[] { new Block(MaterialType.Metal),    new Block(),                        new Block(), new Block() },
        new Block[] { new Block(),                      new Block(),                        new Block(), new Block() }
      };

        public static readonly Block[][] Square = new Block[][] {
        new Block[] { new Block(MaterialType.Metal),    new Block(MaterialType.Organic),    new Block(), new Block() },
        new Block[] { new Block(MaterialType.Metal),    new Block(MaterialType.Organic),    new Block(), new Block() },
        new Block[] { new Block(),                      new Block(),                        new Block(), new Block() },
        new Block[] { new Block(),                      new Block(),                        new Block(), new Block() }
      };

        public static readonly Block[][] MetallicSquare = new Block[][] {
        new Block[] { new Block(MaterialType.Metal),    new Block(MaterialType.Metal),    new Block(), new Block() },
        new Block[] { new Block(MaterialType.Metal),    new Block(MaterialType.Metal),    new Block(), new Block() },
        new Block[] { new Block(),                      new Block(),                        new Block(), new Block() },
        new Block[] { new Block(),                      new Block(),                        new Block(), new Block() }
      };

        public static readonly Block[][] OrganicSquare = new Block[][] {
        new Block[] { new Block(MaterialType.Organic),    new Block(MaterialType.Organic),    new Block(), new Block() },
        new Block[] { new Block(MaterialType.Organic),    new Block(MaterialType.Organic),    new Block(), new Block() },
        new Block[] { new Block(),                      new Block(),                        new Block(), new Block() },
        new Block[] { new Block(),                      new Block(),                        new Block(), new Block() }
      };

        public static readonly Block[][] Fuel = new Block[][] {
        new Block[] { new Block(MaterialType.Fuel),     new Block(),    new Block(), new Block() },
        new Block[] { new Block(),                      new Block(),    new Block(), new Block() },
        new Block[] { new Block(),                      new Block(),    new Block(), new Block() },
        new Block[] { new Block(),                      new Block(),    new Block(), new Block() }
      };

        public static readonly Block[][] L = new Block[][] {
        new Block[] {  new Block(MaterialType.Metal),   new Block(),                    new Block(), new Block() },
        new Block[] {  new Block(MaterialType.Metal),   new Block(),                    new Block(), new Block() },
        new Block[] {  new Block(MaterialType.Organic), new Block(MaterialType.Organic),new Block(), new Block() },
        new Block[] {  new Block(),                     new Block(),                    new Block(), new Block() }
      };

        public static readonly Block[][] ReverseL = new Block[][] {
        new Block[] { new Block(),                      new Block(MaterialType.Metal),   new Block(), new Block() },
        new Block[] { new Block(),                      new Block(MaterialType.Organic), new Block(), new Block() },
        new Block[] { new Block(MaterialType.Organic),  new Block(MaterialType.Organic), new Block(), new Block() },
        new Block[] { new Block(),                      new Block(),                     new Block(), new Block() }
      };

        public static readonly Block[][] T = new Block[][] {
        new Block[] {  new Block(MaterialType.Organic), new Block(),                    new Block(), new Block() },
        new Block[] {  new Block(MaterialType.Metal),   new Block(MaterialType.Metal),  new Block(), new Block() },
        new Block[] {  new Block(MaterialType.Metal), new Block(),                    new Block(), new Block() },
        new Block[] {  new Block(),                     new Block(),                    new Block(), new Block() }
      };

        public static readonly Block[][] Line = new Block[][] {
        new Block[] {  new Block(MaterialType.Organic), new Block(), new Block(), new Block() },
        new Block[] {  new Block(MaterialType.Organic), new Block(), new Block(), new Block() },
        new Block[] {  new Block(MaterialType.Organic), new Block(), new Block(), new Block() },
        new Block[] {  new Block(MaterialType.Metal), new Block(), new Block(), new Block() }
      };

        public static readonly Block[][][] ValidDomino = new Block[][][] {
        Skew,
        ReverseSkew,
        Square,
        L,
        ReverseL,
        T,
        Line
      };

        public static readonly Block[][] SingleBlockOrganic = new Block[][] {
        new Block[] {  new Block(MaterialType.Organic), new Block(), new Block(), new Block() },
        new Block[] { new Block(), new Block(), new Block(), new Block() },
        new Block[] { new Block(), new Block(), new Block(), new Block() },
        new Block[] { new Block(), new Block(), new Block(), new Block() }
      };

        public static readonly Block[][] DoubleBlockOrganic = new Block[][] {
        new Block[] {  new Block(MaterialType.Organic), new Block(), new Block(), new Block() },
        new Block[] {  new Block(MaterialType.Organic), new Block(), new Block(), new Block() },
        new Block[] { new Block(), new Block(), new Block(), new Block() },
        new Block[] { new Block(), new Block(), new Block(), new Block() }
      };

        public static readonly Block[][] TripleBlockOrganic = new Block[][] {
        new Block[] {  new Block(MaterialType.Organic), new Block(), new Block(), new Block() },
        new Block[] {  new Block(MaterialType.Organic),  new Block(MaterialType.Organic), new Block(), new Block() },
        new Block[] { new Block(), new Block(), new Block(), new Block() },
        new Block[] { new Block(), new Block(), new Block(), new Block() }
      };

        public static readonly Block[][] SingleBlockMetallic = new Block[][] {
        new Block[] {  new Block(MaterialType.Organic), new Block(), new Block(), new Block() },
        new Block[] { new Block(), new Block(), new Block(), new Block() },
        new Block[] { new Block(), new Block(), new Block(), new Block() },
        new Block[] { new Block(), new Block(), new Block(), new Block() }
      };

        public static readonly Block[][] DoubleBlockMetallic = new Block[][] {
        new Block[] {  new Block(MaterialType.Organic), new Block(), new Block(), new Block() },
        new Block[] {  new Block(MaterialType.Organic), new Block(), new Block(), new Block() },
        new Block[] { new Block(), new Block(), new Block(), new Block() },
        new Block[] { new Block(), new Block(), new Block(), new Block() }
      };

        public static readonly Block[][] TripleBlockMetallic = new Block[][] {
        new Block[] {  new Block(MaterialType.Organic), new Block(), new Block(), new Block() },
        new Block[] {  new Block(MaterialType.Organic),  new Block(MaterialType.Organic), new Block(), new Block() },
        new Block[] { new Block(), new Block(), new Block(), new Block() },
        new Block[] { new Block(), new Block(), new Block(), new Block() }
      };

        public static readonly Block[][][] DominoPieces = new Block[][][] {
        SingleBlockOrganic,
        DoubleBlockOrganic,
        TripleBlockOrganic
      };

        public static readonly Block[][] None = new Block[][] {
        new Block[] { new Block(), new Block(), new Block(), new Block() },
        new Block[] { new Block(), new Block(), new Block(), new Block() },
        new Block[] { new Block(), new Block(), new Block(), new Block() },
        new Block[] { new Block(), new Block(), new Block(), new Block() }
      };

        #endregion

        public static Block[][] GetRandomValidDomino()
        {
            return ValidDomino[Random.Range(0, ValidDomino.Length)];
        }

        public static Block[][] GetRandomDominoPiece()
        {
            return DominoPieces[Random.Range(0, DominoPieces.Length)];
        }

        public static Block[][] GetMinimumDominoArea(Block[][] domino)
        {
            int minCol = 10000;
            int maxCol = 0;
            int minRow = 10000;
            int maxRow = 0;

            for (int row = 0; row < domino.Length; row++)
            {
                for (int col = 0; col < domino[row].Length; col++)
                {
                    if (domino[row][col] != null)
                    {
                        if (row < minRow) minRow = row;
                        if (row > maxRow) maxRow = row;

                        if (col < minCol) minCol = col;
                        if (col > maxCol) maxCol = col;
                    }
                }
            }

            Block[][] minimumDominoArea = new Block[maxRow - minRow + 1][];

            for (int x = 0; x < minimumDominoArea.Length; x++)
            {
                minimumDominoArea[x] = new Block[maxCol - minCol + 1];
                for (int y = 0; y < minimumDominoArea[x].Length; y++)
                    minimumDominoArea[x][y] = domino[x + minRow][y + minCol];
            }

            return minimumDominoArea;
        }

        public static Trash GetMinimumDominoArea(Trash domino)
        {
            int minCol = 10000;
            int maxCol = 0;
            int minRow = 10000;
            int maxRow = 0;

            bool exists = false;

            for (int row = 0; row < domino.Blocks.Length; row++)
            {
                for (int col = 0; col < domino.Blocks[row].Length; col++)
                {
                    if (domino.Blocks[row][col].Exists)
                    {
                        exists = true;

                        if (row < minRow) minRow = row;
                        if (row > maxRow) maxRow = row;

                        if (col < minCol) minCol = col;
                        if (col > maxCol) maxCol = col;
                    }
                }
            }

            if (!exists)
                return new Trash(None);

            Block[][] minimumDominoArea = new Block[maxRow - minRow + 1][];

            for (int x = 0; x < minimumDominoArea.Length; x++)
            {
                minimumDominoArea[x] = new Block[maxCol - minCol + 1];
                for (int y = 0; y < minimumDominoArea[x].Length; y++)
                    minimumDominoArea[x][y] = domino.Blocks[x + minRow][y + minCol];
            }

            return new Trash()
            {
                Blocks = minimumDominoArea
            };
        }

        public static Block[][] MoveDominoUp(Block[][] domino)
        {
            Block[][] movedDomino = new Block[domino[0].Length][];

            for (int x = 0; x < movedDomino.Length; x++)
            {
                movedDomino[x] = new Block[domino.Length];
            }

            for (int y = 0; y < domino.Length; y++)
            {
                if (domino[0][y].Exists)
                {
                    return domino;
                }
            }

            for (int x = 1; x < domino.Length; x++)
            {
                for (int y = 0; y < domino[x].Length; y++)
                {
                    movedDomino[x - 1][y] = domino[x][y];
                }
            }
            for (int y = 0; y < domino[0].Length; y++)
            {
                movedDomino[domino[0].Length - 1][y] = null;
            }

            return movedDomino;
        }

        public static Block[][] MoveDominoDown(Block[][] domino)
        {
            Block[][] movedDomino = new Block[domino[0].Length][];

            for (int x = 0; x < movedDomino.Length; x++)
            {
                movedDomino[x] = new Block[domino.Length];
            }

            for (int y = 0; y < domino.Length; y++)
            {
                if (domino[domino.Length - 1][y].Exists)
                {
                    return domino;
                }
            }

            for (int x = 0; x < domino.Length - 1; x++)
            {
                for (int y = 0; y < domino[x].Length; y++)
                {
                    movedDomino[x + 1][y] = domino[x][y];
                }
            }
            for (int y = 0; y < domino[0].Length; y++)
            {
                movedDomino[0][y] = null;
            }

            return movedDomino;
        }

        public static Block[][] MoveDominoRight(Block[][] domino)
        {
            Block[][] movedDomino = new Block[domino[0].Length][];

            for (int x = 0; x < movedDomino.Length; x++)
            {
                movedDomino[x] = new Block[domino.Length];
            }

            for (int x = 0; x < domino.Length; x++)
            {
                if (domino[x][domino.Length - 1].Exists)
                {
                    return domino;
                }
            }

            for (int x = 0; x < domino.Length; x++)
            {
                for (int y = 0; y < domino[x].Length - 1; y++)
                {
                    movedDomino[x][y + 1] = domino[x][y];
                }
                movedDomino[x][0] = null;
            }

            return movedDomino;
        }

        public static Block[][] MoveDominoLeft(Block[][] domino)
        {
            Block[][] movedDomino = new Block[domino[0].Length][];

            for (int x = 0; x < movedDomino.Length; x++)
            {
                movedDomino[x] = new Block[domino.Length];
            }

            for (int x = 0; x < domino.Length; x++)
            {
                if (domino[x][0].Exists)
                {
                    return domino;
                }
            }

            for (int x = 0; x < domino.Length; x++)
            {
                for (int y = 0; y < domino[x].Length - 1; y++)
                {
                    movedDomino[x][y] = domino[x][y + 1];
                }
                movedDomino[x][domino.Length - 1] = null;
            }

            return movedDomino;
        }

        public static Block[][] RotateDominoClockwise(Block[][] domino)
        {
            Block[][] rotatedDomino = new Block[domino[0].Length][];

            for (int x = 0; x < rotatedDomino.Length; x++)
            {
                rotatedDomino[x] = new Block[domino.Length];
            }

            for (int x = 0; x < domino.Length; x++)
            {
                for (int y = 0; y < domino[x].Length; y++)
                {
                    rotatedDomino[y][domino.Length - 1 - x] = domino[x][y];
                }
            }

            return rotatedDomino;
        }

        public static Block[][] RotateDominoCounterClockwise(Block[][] domino)
        {
            Block[][] rotatedDomino = new Block[domino[0].Length][];

            //Créer un tableau de la même taille
            for (int x = 0; x < rotatedDomino.Length; x++)
            {
                rotatedDomino[x] = new Block[domino.Length];
            }


            for (int x = 0; x < domino.Length; x++)
            {
                for (int y = 0; y < domino[x].Length; y++)
                {
                    rotatedDomino[domino.Length - 1 - y][x] = domino[x][y];
                }
            }

            return rotatedDomino;
        }


        public static bool CompareAllDominosRotations(Block[][] domino1, Block[][] domino2)
        {
            var rotatedDomino1 = RotateDominoClockwise(domino1);
            var rotatedDomino2 = RotateDominoClockwise(rotatedDomino1);
            var rotatedDomino3 = RotateDominoClockwise(rotatedDomino2);

            return CompareDominos(domino1, domino2) ||
              CompareDominos(rotatedDomino1, domino2) ||
              CompareDominos(rotatedDomino2, domino2) ||
              CompareDominos(rotatedDomino3, domino2);
        }

        public static bool CompareDominos(Block[][] domino1, Block[][] domino2)
        {
            var minDomino1 = GetMinimumDominoArea(domino1);
            var minDomino2 = GetMinimumDominoArea(domino2);

            if (minDomino1.Length == minDomino2.Length && minDomino1[0].Length == minDomino2[0].Length)
            {
                for (int x = 0; x < minDomino1.Length; x++)
                    for (int y = 0; y < minDomino1[x].Length; y++)
                        if (minDomino1[x][y] != minDomino2[x][y])
                            return false;

                return true;
            }


            return false;
        }

        public static List<Block[][]> SeperateMaterial(Block[][] trash)
        {
            List<Block[][]> trashList = new List<Block[][]>();

            Block[][] materialMetal = new Block[trash.Length][];
            Block[][] materialOrganic = new Block[trash.Length][];

            for (int x = 0; x < trash.Length; x++)
            {
                materialMetal[x] = new Block[trash[x].Length];
                materialOrganic[x] = new Block[trash[x].Length];

                for (int y = 0; y < trash[x].Length; y++)
                {
                    switch (trash[x][y].Material)
                    {
                        case MaterialType.Metal:
                            materialMetal[x][y] = trash[x][y];
                            materialOrganic[x][y] = new Block();
                            break;
                        case MaterialType.Organic:
                            materialOrganic[x][y] = trash[x][y];
                            materialMetal[x][y] = new Block();
                            break;
                    }
                }
            }

            trashList.Add(materialMetal);
            trashList.Add(materialOrganic);

            return trashList;
        }

        public static List<Trash> SeparateTrash(Trash trash)
        {
            List<Trash> list = new List<Trash>();

            var blocks = SeperateMaterial(trash.Blocks);

            foreach (var block in blocks)
            {
                list.Add(new Trash(block));
            }

            return list;
        }

        //public static Block isDominoFullfillingRequest(Trash domino, DominoRequest dominoRequest)
        //{
        //  var isCorrectShape = CompareAllDominosRotations(domino.GetBlocksAsBlocks(), dominoRequest.Blocks);

        //  if (!isCorrectShape) return null;

        //  var isCorrectColor = new Block(MaterialType.Organic);

        //  for (int x = 0; x < domino.Blocks.Length; x++)
        //      for (int y = 0; y < domino.Blocks[x].Length; y++)
        //          if (domino.Blocks[x][y].Exists && domino.Blocks[x][y].Color != dominoRequest.Color)
        //              isCorrectColor = null;

        //  return isCorrectShape && isCorrectColor;
        //}

        public static BlockColor GetRandomColor()
        {
            var randomColor = Random.Range(2, BlockColor.GetNames(typeof(BlockColor)).Length);
            return (BlockColor)randomColor;
        }

    }
}


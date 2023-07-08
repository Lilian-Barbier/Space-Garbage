using Enums;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using static Unity.Collections.AllocatorManager;

public class AssemblerManager : MonoBehaviour
{
    //voir pour partager ça avec GameManager
    [SerializeField] private Sprite blockSprite;
    private Color blueOutline = new Color(27 / 255f, 33 / 255f, 114 / 255f, 1);

    [SerializeField] GameObject dominoPrefab;

    Domino currentDomino;
    SpriteRenderer spriteRendererCreatedDomino;
    SpriteRenderer spriteRendererAddedDomino;

    // Start is called before the first frame update
    void Start()
    {
        //Voir pour passer via un tag ?
        spriteRendererCreatedDomino = GetComponentsInChildren<SpriteRenderer>()[1];
        spriteRendererAddedDomino = GetComponentsInChildren<SpriteRenderer>()[2];
        spriteRendererCreatedDomino.color = Color.white;

        currentDomino = GetEmptyDomino();
        var currentColor = spriteRendererCreatedDomino.color;
        currentColor.a = 1;
        spriteRendererCreatedDomino.color = currentColor;
        CreateSprite();
    }

    Domino GetEmptyDomino()
    {
        var emptyDominoTable = new bool[4][];

        for (var i = 0; i < 4; i++)
        {
            emptyDominoTable[i] = new bool[4];
            for (var j = 0; j < 4; j++)
            {
                emptyDominoTable[i][j] = false;
            }
        }
        return new Domino(emptyDominoTable);
    }

    public GameObject GetDomino()
    {
        var newDomino = Instantiate(dominoPrefab);
        newDomino.GetComponent<DominoBehavior>().domino = currentDomino;
        currentDomino = GetEmptyDomino();
        CreateSprite();
        return newDomino;
    }

    public bool CanAddDomino(Domino domino)
    {
        for (var i = 0; i < 4; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                if (currentDomino.Blocks[i][j].Exists && domino.Blocks[i][j].Exists)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void AddDomino(Domino domino)
    {
        for (var i = 0; i < 4; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                if (!currentDomino.Blocks[i][j].Exists)
                {
                    currentDomino.Blocks[i][j] = domino.Blocks[i][j];
                }
            }
        }
        spriteRendererAddedDomino.sprite = null;
        CreateSprite();
    }

    void CreateSprite()
    {
        spriteRendererCreatedDomino.sprite = GenerateAssemblerDominoSprite(currentDomino);
    }

    public void CreateSpriteForAddedDomino(Domino domino)
    {
        var currentColor = spriteRendererAddedDomino.color;
        currentColor.a = 0.7f;
        spriteRendererAddedDomino.color = currentColor;
        spriteRendererAddedDomino.sprite = GenerateAssemblerDominoSprite(domino);
    }


    Sprite GenerateAssemblerDominoSprite(Domino domino)
    {
        DominoUtils.PrintDomino(domino.GetBlocksAsBools());

        int height = (domino.Blocks.Length + 1) * 6;
        int width = domino.Blocks[0].Length * 7;

        Resources.UnloadUnusedAssets();
        Color transparentColor = new Color(0, 0, 0, 0);

        var newTexture = new Texture2D(32, 32);

        // INIT BACKGROUND

        for (var x = 0; x < 32; x++)
            for (var y = 0; y < 32; y++)
                newTexture.SetPixel(x, y, transparentColor);

        // DRAW BLOCKS

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                int xPosInArray = (int)Mathf.Floor(x / 7);
                int yPosInArray = (int)Mathf.Floor(y / 6);

                int xPosInsideCell = x % 7;
                int yPosInsideCell = y % 6;
                Color pixelColor = transparentColor;

                if (
                  yPosInArray == domino.Blocks.Length ||
                  xPosInArray == domino.Blocks[yPosInArray].Length ||
                  !domino.Blocks[yPosInArray][xPosInArray].Exists
                )
                {

                    var previousYPosInArray = yPosInArray - 1;

                    if (
                      previousYPosInArray < 0 ||
                      !domino.Blocks[previousYPosInArray][xPosInArray].Exists ||
                      yPosInsideCell == 5
                    ) continue;

                    pixelColor = blockSprite.texture.GetPixel(xPosInsideCell, 4 - yPosInsideCell);
                    newTexture.SetPixel(x + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y, pixelColor);

                    continue;
                }

                // DRAW OUTLINE

                var shouldPlaceLeftOutline = xPosInsideCell == 0 && (xPosInArray == 0 || !domino.Blocks[yPosInArray][xPosInArray - 1].Exists);
                var shouldPlaceRightOutline = xPosInsideCell == 6 && (xPosInArray == domino.Blocks[yPosInArray].Length - 1 || !domino.Blocks[yPosInArray][xPosInArray + 1].Exists);
                var shouldPlaceTopOutline = yPosInsideCell == 0 && (yPosInArray == 0 || !domino.Blocks[yPosInArray - 1][xPosInArray].Exists);
                var shouldPlaceBottomOutline = yPosInsideCell == 5 && (yPosInArray == domino.Blocks.Length - 1 || !domino.Blocks[yPosInArray + 1][xPosInArray].Exists);

                if (shouldPlaceLeftOutline)
                    newTexture.SetPixel(x - 1 + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y, blueOutline);

                if (shouldPlaceRightOutline)
                    newTexture.SetPixel(x + 1 + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y, blueOutline);

                if (shouldPlaceTopOutline)
                    newTexture.SetPixel(x + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y + 1, blueOutline);

                if (shouldPlaceBottomOutline)
                    newTexture.SetPixel(x + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y - 6, blueOutline);

                if (shouldPlaceTopOutline && shouldPlaceLeftOutline)
                    newTexture.SetPixel(x - 1 + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y + 1, blueOutline);

                if (shouldPlaceTopOutline && shouldPlaceRightOutline)
                    newTexture.SetPixel(x + 1 + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y + 1, blueOutline);

                if (shouldPlaceBottomOutline && shouldPlaceLeftOutline)
                    for (var i = 0; i < 6; i++)
                        newTexture.SetPixel(x - 1 + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y - 1 - i, blueOutline);

                if (shouldPlaceBottomOutline && shouldPlaceRightOutline)
                    for (var i = 0; i < 6; i++)
                        newTexture.SetPixel(x + 1 + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y - 1 - i, blueOutline);

                // END DRAW OUTLINE

                pixelColor = blockSprite.texture.GetPixel(xPosInsideCell, 10 - yPosInsideCell);
                newTexture.SetPixel(x + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y, pixelColor);
            }
        }

        newTexture.filterMode = FilterMode.Point;
        newTexture.wrapMode = TextureWrapMode.Clamp;
        newTexture.alphaIsTransparency = true;

        newTexture.Apply();

        var finalSprite = Sprite.Create(newTexture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
        finalSprite.name = "DominoSprite";
        return finalSprite;
    }
}

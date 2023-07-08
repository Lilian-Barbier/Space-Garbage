using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class GameManager : MonoBehaviour
{
  [SerializeField] private Sprite blockSprite;
  private Color blueOutline = new Color(27/255f, 33/255f, 114/255f, 1);
  
  DominoRequest[] dominoRequestList;
  
  float timeSinceLastBlockRequest;
  float maxTimeBetweenBlockRequests = 60f;

  void Start()
  {

  }

  void Update()
  {
      
  }

  void FixedUpdate() 
  {
    // if(dominoRequestList.Length == 0)
    // {
    //   // timeSinceLastBlockRequest += Time.deltaTime;
    //   // if(timeSinceLastBlockRequest >= maxTimeBetweenBlockRequests)
    //   // {
    //   //   timeSinceLastBlockRequest = 0f;
    //   //   BlockRequestList = GenerateBlockRequestList();
    //   // }
    // }
  }

  public Sprite GenerateDominoSprite(Domino domino)
  { 
    var minArea = DominoManager.GetMinimumDominoArea(domino);

    DominoManager.PrintDomino(minArea.GetBlocksAsBools());

    int height = (minArea.Blocks.Length + 1) * 6;
    int width = minArea.Blocks[0].Length * 7;

    Resources.UnloadUnusedAssets();
    Color transparentColor = new Color(0, 0, 0, 0);

    var newTexture = new Texture2D(32, 32);

    // INIT BACKGROUND

    for (var x = 0; x < 32; x++)
      for (var y = 0; y < 32; y++)
        newTexture.SetPixel(x, y, transparentColor);

    // DRAW BLOCKS

    for (var x = 0; x < width; x++) {
      for (var y = 0; y < height; y++)
      {
        int xPosInArray = (int) Mathf.Floor(x / 7);
        int yPosInArray = (int) Mathf.Floor(y / 6);

        int xPosInsideCell = x % 7;
        int yPosInsideCell = y % 6;
        Color pixelColor = transparentColor;

        if (
          yPosInArray == minArea.Blocks.Length ||
          xPosInArray == minArea.Blocks[yPosInArray].Length ||
          !minArea.Blocks[yPosInArray][xPosInArray].Exists
        ) {
          
          var previousYPosInArray = yPosInArray - 1;

          if (
            previousYPosInArray < 0 ||
            !minArea.Blocks[previousYPosInArray][xPosInArray].Exists ||
            yPosInsideCell == 5
          ) continue;

          pixelColor = blockSprite.texture.GetPixel(xPosInsideCell, 4 - yPosInsideCell);
          newTexture.SetPixel(x + (int) Mathf.Floor((32 - width)/2), 31 - (int) Mathf.Floor((32 - height)/2) - y, pixelColor);
          
          continue;
        }

        // DRAW OUTLINE

        var shouldPlaceLeftOutline = xPosInsideCell == 0 && (xPosInArray == 0 || !minArea.Blocks[yPosInArray][xPosInArray - 1].Exists);
        var shouldPlaceRightOutline = xPosInsideCell == 6 && (xPosInArray == minArea.Blocks[yPosInArray].Length - 1 || !minArea.Blocks[yPosInArray][xPosInArray + 1].Exists);
        var shouldPlaceTopOutline = yPosInsideCell == 0 && (yPosInArray == 0 || !minArea.Blocks[yPosInArray - 1][xPosInArray].Exists);
        var shouldPlaceBottomOutline = yPosInsideCell == 5 && (yPosInArray == minArea.Blocks.Length - 1 || !minArea.Blocks[yPosInArray + 1][xPosInArray].Exists);

        if (shouldPlaceLeftOutline)
          newTexture.SetPixel(x - 1 + (int) Mathf.Floor((32 - width)/2), 31 - (int) Mathf.Floor((32 - height)/2) - y, blueOutline);

        if (shouldPlaceRightOutline)
          newTexture.SetPixel(x + 1 + (int) Mathf.Floor((32 - width)/2), 31 - (int) Mathf.Floor((32 - height)/2) - y, blueOutline);

        if (shouldPlaceTopOutline)
          newTexture.SetPixel(x + (int) Mathf.Floor((32 - width)/2), 31 - (int) Mathf.Floor((32 - height)/2) - y + 1, blueOutline);

        if (shouldPlaceBottomOutline)
          newTexture.SetPixel(x + (int) Mathf.Floor((32 - width)/2), 31 - (int) Mathf.Floor((32 - height)/2) - y - 6, blueOutline);

        if (shouldPlaceTopOutline && shouldPlaceLeftOutline)
          newTexture.SetPixel(x - 1 + (int) Mathf.Floor((32 - width)/2), 31 - (int) Mathf.Floor((32 - height)/2) - y + 1, blueOutline);

        if (shouldPlaceTopOutline && shouldPlaceRightOutline)
          newTexture.SetPixel(x + 1 + (int) Mathf.Floor((32 - width)/2), 31 - (int) Mathf.Floor((32 - height)/2) - y + 1, blueOutline);

        if (shouldPlaceBottomOutline && shouldPlaceLeftOutline)
          for (var i = 0; i < 6; i++)
            newTexture.SetPixel(x - 1 + (int) Mathf.Floor((32 - width)/2), 31 - (int) Mathf.Floor((32 - height)/2) - y - 1 - i, blueOutline);

        if (shouldPlaceBottomOutline && shouldPlaceRightOutline)
          for (var i = 0; i < 6; i++)
            newTexture.SetPixel(x + 1 + (int) Mathf.Floor((32 - width)/2), 31 - (int) Mathf.Floor((32 - height)/2) - y - 1 - i, blueOutline);

        // END DRAW OUTLINE

        pixelColor = blockSprite.texture.GetPixel(xPosInsideCell, 10 - yPosInsideCell);
        newTexture.SetPixel(x + (int) Mathf.Floor((32 - width)/2), 31 - (int) Mathf.Floor((32 - height)/2) - y, pixelColor);
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

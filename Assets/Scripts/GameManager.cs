using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Sprite blockSprite;
    [SerializeField] private Color blueOutline = new Color(27, 33, 114, 1);
    [SerializeField] private SpriteRenderer blockSpriteRenderer;
    
    DominoRequest[] dominoRequestList;
    
    float timeSinceLastBlockRequest;
    float maxTimeBetweenBlockRequests = 60f;



    void Start()
    {
        var dominoShape = DominoManager.GetRandomValidDomino();
        var domino = new Domino(dominoShape);
        
        blockSpriteRenderer.sprite = GenerateDominoSprite(domino);
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

    private Sprite GenerateDominoSprite(Domino domino)
    { 
      var minArea = DominoManager.GetMinimumDominoArea(domino);

      int height = (minArea.Blocks.Length + 1) * 6;
      int width = minArea.Blocks[0].Length * 7;

      Resources.UnloadUnusedAssets();
      Color transparentColor = new Color(0, 0, 0, 1);

      var newTexture = new Texture2D(32, 32);

      // INIT BACKGROUND

      for (var x = 0; x < 32; x++)
        for (var y = 0; y < 32; y++)
          newTexture.SetPixel(x, y, transparentColor);

      // DRAW OUTLINE



      // DRAW BLOCKS

      for (var x = 0; x < width; x++) {
        for (var y = 0; y < height; y++)
        {
          int xPosInArray = (int) Mathf.Floor(x / 7);
          int yPosInArray = (int) Mathf.Floor(y / 6);

          int xPosInsideCell = x % 7;
          int yPosInsideCell = y % 6;
          Color pixelColor = transparentColor;

          if(
            yPosInArray == minArea.Blocks.Length ||
            xPosInArray == minArea.Blocks[yPosInArray].Length ||
            !minArea.Blocks[yPosInArray][xPosInArray].Exists
          ) {
            
            var previousYPosInArray = yPosInArray - 1;

            if (
              previousYPosInArray < 0 
              || !minArea.Blocks[previousYPosInArray][xPosInArray].Exists
              || yPosInsideCell == 5
            ) continue;

            pixelColor = blockSprite.texture.GetPixel(xPosInsideCell, 4 - yPosInsideCell);
            newTexture.SetPixel(x + (int) Mathf.Floor((32 - width)/2), 31 - (int) Mathf.Floor((32 - height)/2) - y, pixelColor);
            
            continue;
          }

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

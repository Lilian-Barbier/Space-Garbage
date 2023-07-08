using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Sprite blockSprite;
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
      Resources.UnloadUnusedAssets();
      Color transparentColor = new Color(0, 0, 0, 0);

      var newTexture = new Texture2D(32, 32);

      for (var x = 0; x < 32; x++)
        for (var y = 0; y < 32; y++)
          newTexture.SetPixel(x, y, transparentColor);

      for (var x = 0; x < 28; x++) {
        for (var y = 0; y < 24; y++)
        {
          int xPosInArray = (int) Mathf.Floor(x / 7);
          int yPosInArray = (int) Mathf.Floor(y / 6);

          int xPosInsideCell = x % 7;
          int yPosInsideCell = y % 6;
          Color pixelColor = transparentColor;

          if(!domino.Blocks[yPosInArray][xPosInArray].Exists) {
            
            var previousYPosInArray = yPosInArray - 1;

            if (
              previousYPosInArray < 0 
              || !domino.Blocks[previousYPosInArray][xPosInArray].Exists
              || yPosInsideCell == 5
            ) continue;

            pixelColor = blockSprite.texture.GetPixel(xPosInsideCell, 4 - yPosInsideCell);
            newTexture.SetPixel(x + 2, 27 - y, pixelColor);
            
            continue;
          }

          pixelColor = blockSprite.texture.GetPixel(xPosInsideCell, 10 - yPosInsideCell);
          newTexture.SetPixel(x + 2, 27 - y, pixelColor);
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

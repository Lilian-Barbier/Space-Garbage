using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Sprite blockSprite;
    
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
      if(dominoRequestList.Length == 0)
      {
        // timeSinceLastBlockRequest += Time.deltaTime;
        // if(timeSinceLastBlockRequest >= maxTimeBetweenBlockRequests)
        // {
        //   timeSinceLastBlockRequest = 0f;
        //   BlockRequestList = GenerateBlockRequestList();
        // }
      }
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

          if(!domino.Blocks[xPosInArray][yPosInArray].Exists) {
            continue;
          }

          int xPosInsideCell = x % 7;
          int yPosInsideCell = y % 6;

          var pixelColor = blockSprite.texture.GetPixel(xPosInsideCell, yPosInsideCell);

          newTexture.SetPixel(x+2, y+4, pixelColor);
        }
      }

      newTexture.Apply();

      var finalSprite = Sprite.Create(newTexture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
      finalSprite.name = "DominoSprite";
      return finalSprite;
    }
}

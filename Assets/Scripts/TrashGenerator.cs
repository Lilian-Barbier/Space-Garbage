using UnityEngine;
using Utils;
using Models;
using Assets.Scripts.Enums;

public class TrashGenerator : MonoBehaviour
{
    [SerializeField] private Sprite defaultBlockSprite;
    [SerializeField] private Sprite redBlockSprite;
    [SerializeField] private Sprite greenBlockSprite;
    [SerializeField] private Sprite blueBlockSprite;
    [SerializeField] private Sprite yellowBlockSprite;
    [SerializeField] private Sprite cyanBlockSprite;
    [SerializeField] private Sprite purpleBlockSprite;
    [SerializeField] private Sprite blackBlockSprite;

    private static readonly int spriteSize = 256;
    private static readonly uint spritePadding = (uint)Mathf.Floor(1.25f * (spriteSize / 32));
    private static readonly int blockPixelSize = spriteSize / 32;
    private static readonly int blockSizeY = 6 * blockPixelSize;
    private static readonly int blockSizeX = 7 * blockPixelSize;

    private static readonly int blockSideSize = 5 * blockPixelSize;
    private static readonly int fullBlockHeight = blockSizeY + blockSideSize;

    public Sprite GenerateTrashSprite(Trash domino)
    {
        var minArea = TrashUtils.GetMinimumDominoArea(domino);

        int dominoPixelHeight = (minArea.Blocks.Length + 1) * blockSizeY;
        int dominoPixelWidth = minArea.Blocks[0].Length * blockSizeX;

        int dominoPaddingLeft = (int)Mathf.Floor((spriteSize - dominoPixelWidth) / 2);
        int dominoPaddingBottom = (int)Mathf.Floor((spriteSize - dominoPixelHeight) / 2);

        Resources.UnloadUnusedAssets();
        Color transparent = new Color(0, 0, 0, 0);

        var newTexture = new Texture2D(spriteSize, spriteSize);

        // INIT BACKGROUND

        for (var x = 0; x < spriteSize; x++)
            for (var y = 0; y < spriteSize; y++)
                newTexture.SetPixel(x, y, transparent);

        // DRAW BLOCKS

        for (var blockY = 0; blockY < minArea.Blocks.Length; blockY++)
        {
            for (var blockX = 0; blockX < minArea.Blocks[blockY].Length; blockX++)
            {

                if (!minArea.Blocks[blockY][blockX].Exists) continue;

                var blockSprite = GetSpriteFromMaterial(minArea.Blocks[blockY][blockX].Material);

                for (var x = 0; x < blockSizeX; x++)
                {
                    for (var y = 0; y < blockSizeY; y++)
                    {
                        var pixelColor = blockSprite.texture.GetPixel(x, fullBlockHeight - 1 - y);
                        if (pixelColor.a == 0) continue;

                        if (pixelColor.a == 1)
                            newTexture.SetPixel(x + dominoPaddingLeft + blockX * blockSizeX, spriteSize - 1 - dominoPaddingBottom - blockY * blockSizeY - y, pixelColor);
                        else
                        { // if the texture is semi-transparent, we need to merge the colors instead of replacing them
                            var mergedColor = Color.Lerp(newTexture.GetPixel(x + dominoPaddingLeft + blockX * blockSizeX, spriteSize - 1 - dominoPaddingBottom - blockY * blockSizeY - y), pixelColor, pixelColor.a);
                            newTexture.SetPixel(x + dominoPaddingLeft + blockX * blockSizeX, spriteSize - 1 - dominoPaddingBottom - blockY * blockSizeY - y, mergedColor);
                        }
                    }
                }

                for (var x = 0; x < blockSizeX; x++)
                {
                    for (var y = 0; y < blockSideSize; y++)
                    {
                        var pixelColor = blockSprite.texture.GetPixel(x, fullBlockHeight - blockSizeY - 1 - y);
                        if (pixelColor.a != 0)
                            newTexture.SetPixel(x + dominoPaddingLeft + blockX * blockSizeX, spriteSize - 1 - dominoPaddingBottom - (blockY + 1) * blockSizeY - y, pixelColor);
                    }
                }
            }
        }

        // CONFIG TEXTURE & SPRITE

        newTexture.filterMode = FilterMode.Point;
        newTexture.wrapMode = TextureWrapMode.Clamp;

        newTexture.Apply();

        var finalSprite = Sprite.Create(newTexture, new Rect(0, 0, spriteSize, spriteSize), new Vector2(0.5f, 0.5f), spriteSize);
        finalSprite.name = "DominoSprite";
        return finalSprite;
    }

    private Sprite GetSpriteFromMaterial(MaterialType color)
    {
        switch (color)
        {
            case MaterialType.Organic:
                return greenBlockSprite;
            case MaterialType.Metal:
                return defaultBlockSprite;
            case MaterialType.Fuel:
                return blackBlockSprite;
            default:
                return defaultBlockSprite;
        }
    }
}

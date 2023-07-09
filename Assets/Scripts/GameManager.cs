using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Utils;
using Enums;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int life = 3;
    [SerializeField] public float dominoRequestDuration = 40f;

    [SerializeField] private Sprite defaultBlockSprite;
    [SerializeField] private Sprite blueBlockSprite;
    [SerializeField] private Sprite lightBlueBlockSprite;
    [SerializeField] private Sprite redBlockSprite;
    [SerializeField] private Sprite lightRedBlockSprite;
    [SerializeField] private Sprite blackBlockSprite;

    private float minTimeBetweenDominoRequests = 12f;
    private float maxTimeBetweenDominoRequests = 36f;

    private Color blueOutline = new Color(27 / 255f, 33 / 255f, 114 / 255f, 1);
    private Color lightBlueOutline = new Color(51 / 255f, 57 / 255f, 132 / 255f, 1);
    private Color redOutline = new Color(82 / 255f, 1 / 255f, 1 / 255f, 1);
    private Color lightRedOutline = new Color(117 / 255f, 11 / 255f, 11 / 255f, 1);
    private Color defaultOutline = new Color(24 / 255f, 24 / 255f, 24 / 255f, 1);
    private Color blackOutline = Color.black;

    // UI Management

    [SerializeField] private GameObject requestPrefab;

    [SerializeField] private GameObject HudCanvas;

    private List<GameObject> hudRequestList;

    private List<TetrisPlayer> playerList;

    public List<DominoRequest> dominoRequestList;

    float timeSinceLastBlockRequest;
    float timeBeforeNextBlockRequest;

    HearthManager hearthManager;
    private int score;

    void Start()
    {
        // hearthManager = FindObjectOfType<HearthManager>().GetComponent<HearthManager>();

        HudCanvas = GameObject.Find("HUD");

        timeSinceLastBlockRequest = 0f;

        dominoRequestList = new List<DominoRequest>();
        hudRequestList = new List<GameObject>();

        playerList = GetRandomPlayers();
    }

    void FixedUpdate()
    {
        // hearthManager.ScoreChanged(score);
        DecreaseDominoRequestTimeList();
        CheckForNewDominoRequest();
        DeleteUnsuccessfulDominoRequests();
    }

    private void DecreaseDominoRequestTimeList()
    {
        for (var i = 0; i < dominoRequestList.Count; i++)
            dominoRequestList[i].RemainingTime -= Time.deltaTime;
    }

    private void CheckForNewDominoRequest()
    {
        timeSinceLastBlockRequest += Time.deltaTime;

        // Skip if there are already 10 requests or no players left
        if (dominoRequestList.Count >= 10 || playerList.Count == 0)
        {
            timeSinceLastBlockRequest = 0f;
            return;
        }

        if (timeBeforeNextBlockRequest < timeSinceLastBlockRequest || dominoRequestList.Count == 0)
            AddRandomDominoRequest();
    }

    private void DeleteUnsuccessfulDominoRequests()
    {
        for (var i = dominoRequestList.Count - 1; i > -1; i--)
        {
            if (dominoRequestList[i].RemainingTime < 0)
            {
                life--;
                // hearthManager.LifeChanged(life);
                if (life <= 0)
                {
                    //Todo Load GameOver scene
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }

                DeleteDominoRequest(i);
            }
        }
    }

    public void DeleteDominoRequest(int i)
    {
        var player = dominoRequestList[i].Player;
        playerList.Add(player);

        dominoRequestList.RemoveAt(i);

        Destroy(hudRequestList[i]);
        hudRequestList.RemoveAt(i);

        for (var j = i; j < hudRequestList.Count; j++)
        {
            var requestRectTransform = hudRequestList[j].GetComponent<RectTransform>();
            requestRectTransform.anchoredPosition = new Vector2(56 + 166 * j, 0);
        }
    }

    private void AddRandomDominoRequest()
    {
        //Todo � retirer, juste pour tester
        if(dominoRequestDuration>26)
            dominoRequestDuration -= 6;

        if (maxTimeBetweenDominoRequests > 15)
            maxTimeBetweenDominoRequests -= 3;

        timeSinceLastBlockRequest = 0f;
        timeBeforeNextBlockRequest = Random.Range(minTimeBetweenDominoRequests, maxTimeBetweenDominoRequests);

        var playerIndex = Random.Range(0, playerList.Count);
        var player = playerList[playerIndex];
        playerList.RemoveAt(playerIndex);

        var dominoRequest = new DominoRequest()
        {
            Blocks = DominoUtils.GetRandomValidDomino(),
            Color = DominoUtils.GetRandomColor(),
            Player = player,
            RemainingTime = dominoRequestDuration,
        };

        // Debug.Log("Adding new domino request: " + dominoRequest.Player.Name + " " + dominoRequest.Player.Age + " " + dominoRequest.Color + "\n" + DominoUtils.PrintDomino(dominoRequest.Blocks));

        dominoRequestList.Add(dominoRequest);
        AddDominoRequestToHUD(dominoRequest);
    }

    private void AddDominoRequestToHUD(DominoRequest dominoRequest)
    {
        var requestGameObject = Instantiate(requestPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        requestGameObject.transform.SetParent(HudCanvas.transform, false);

        var requestRectTransform = requestGameObject.GetComponent<RectTransform>();
        requestRectTransform.anchoredPosition = new Vector2(56 + 166 * (dominoRequestList.Count - 1), 0);

        var requestBehavior = requestGameObject.GetComponent<RequestBehavior>();
        requestBehavior.SetDominoRequest(dominoRequest);

        hudRequestList.Add(requestGameObject);
    }

    private List<TetrisPlayer> GetRandomPlayers()
    {
        var playerList = new List<TetrisPlayer>();

        for (var i = 0; i < 10; i++)
        {
            var player = new TetrisPlayer()
            {
                Name = RequestUtils.GetRandomPlayerName(),
                Age = RequestUtils.GetRandomPlayerAge(),
            };

            playerList.Add(player);
        }

        return playerList;
    }

    public Sprite GenerateDominoSprite(Domino domino)
    {
        var minArea = DominoUtils.GetMinimumDominoArea(domino);

        DominoUtils.PrintDomino(minArea.GetBlocksAsBools());

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

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                int xPosInArray = (int)Mathf.Floor(x / 7);
                int yPosInArray = (int)Mathf.Floor(y / 6);

                int xPosInsideCell = x % 7;
                int yPosInsideCell = y % 6;

                Sprite blockSprite;
                Color pixelColor = transparentColor;
                Color outlineColor = transparentColor;

                if (
                  yPosInArray == minArea.Blocks.Length ||
                  xPosInArray == minArea.Blocks[yPosInArray].Length ||
                  !minArea.Blocks[yPosInArray][xPosInArray].Exists
                )
                {

                    var previousYPosInArray = yPosInArray - 1;

                    if (
                      previousYPosInArray < 0 ||
                      !minArea.Blocks[previousYPosInArray][xPosInArray].Exists ||
                      yPosInsideCell == 5
                    ) continue;

                    switch (minArea.Blocks[previousYPosInArray][xPosInArray].Color)
                    {
                        case BlockColor.Blue:
                            blockSprite = blueBlockSprite;
                            break;
                        case BlockColor.LightBlue:
                            blockSprite = lightBlueBlockSprite;
                            break;
                        case BlockColor.Red:
                            blockSprite = redBlockSprite;
                            break;
                        case BlockColor.LightRed:
                            blockSprite = lightRedBlockSprite;
                            break;
                        case BlockColor.Failed:
                            blockSprite = blackBlockSprite;
                            break;
                        default:
                            blockSprite = defaultBlockSprite;
                            break;
                    }

                    pixelColor = blockSprite.texture.GetPixel(xPosInsideCell, 4 - yPosInsideCell);
                    newTexture.SetPixel(x + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y, pixelColor);

                    continue;
                }

                // DRAW OUTLINE

                switch (minArea.Blocks[yPosInArray][xPosInArray].Color)
                {
                    case BlockColor.Blue:
                        blockSprite = blueBlockSprite;
                        outlineColor = blueOutline;
                        break;
                    case BlockColor.LightBlue:
                        blockSprite = lightBlueBlockSprite;
                        outlineColor = lightBlueOutline;
                        break;
                    case BlockColor.Red:
                        blockSprite = redBlockSprite;
                        outlineColor = redOutline;
                        break;
                    case BlockColor.LightRed:
                        blockSprite = lightRedBlockSprite;
                        outlineColor = lightRedOutline;
                        break;
                    case BlockColor.Failed:
                        blockSprite = blackBlockSprite;
                        outlineColor = blackOutline;
                        break;
                    default:
                        blockSprite = defaultBlockSprite;
                        outlineColor = defaultOutline;
                        break;
                }


                var shouldPlaceLeftOutline = xPosInsideCell == 0 && (xPosInArray == 0 || !minArea.Blocks[yPosInArray][xPosInArray - 1].Exists);
                var shouldPlaceRightOutline = xPosInsideCell == 6 && (xPosInArray == minArea.Blocks[yPosInArray].Length - 1 || !minArea.Blocks[yPosInArray][xPosInArray + 1].Exists);
                var shouldPlaceTopOutline = yPosInsideCell == 0 && (yPosInArray == 0 || !minArea.Blocks[yPosInArray - 1][xPosInArray].Exists);
                var shouldPlaceBottomOutline = yPosInsideCell == 5 && (yPosInArray == minArea.Blocks.Length - 1 || !minArea.Blocks[yPosInArray + 1][xPosInArray].Exists);

                if (shouldPlaceLeftOutline)
                    newTexture.SetPixel(x - 1 + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y, outlineColor);

                if (shouldPlaceRightOutline)
                    newTexture.SetPixel(x + 1 + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y, outlineColor);

                if (shouldPlaceTopOutline)
                    newTexture.SetPixel(x + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y + 1, outlineColor);

                if (shouldPlaceBottomOutline)
                    newTexture.SetPixel(x + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y - 6, outlineColor);

                if (shouldPlaceTopOutline && shouldPlaceLeftOutline)
                    newTexture.SetPixel(x - 1 + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y + 1, outlineColor);

                if (shouldPlaceTopOutline && shouldPlaceRightOutline)
                    newTexture.SetPixel(x + 1 + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y + 1, outlineColor);

                if (shouldPlaceBottomOutline && shouldPlaceLeftOutline)
                    for (var i = 0; i < 6; i++)
                        newTexture.SetPixel(x - 1 + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y - 1 - i, outlineColor);

                if (shouldPlaceBottomOutline && shouldPlaceRightOutline)
                    for (var i = 0; i < 6; i++)
                        newTexture.SetPixel(x + 1 + (int)Mathf.Floor((32 - width) / 2), 31 - (int)Mathf.Floor((32 - height) / 2) - y - 1 - i, outlineColor);

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

    public void GainScore()
    {
        score += 100;
    }

    public void LooseScore()
    {
        score -= 50;
    }

}

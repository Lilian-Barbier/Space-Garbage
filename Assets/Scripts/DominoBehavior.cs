using Models;
using UnityEngine;
using Utils;

public class DominoBehavior : MonoBehaviour
{
    // get own SpriteRenderer
    SpriteRenderer spriteRenderer;
    GameManager gameManager;
    Domino domino;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();

        domino = new Domino(DominoUtils.GetRandomValidDomino());
        SetSpriteAndCollider();
    }

    public void RotateDominoClockwise()
    {
        domino.Blocks = DominoUtils.RotateDominoClockwise(domino.Blocks);
        SetSpriteAndCollider();
    }

    public void RotateDominoCounterClockwise()
    {
        domino.Blocks = DominoUtils.RotateDominoCounterClockwise(domino.Blocks);
        SetSpriteAndCollider();
    }

    void SetSpriteAndCollider()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = gameManager.GenerateDominoSprite(domino);

        Sprite sprite = spriteRenderer.sprite;
        BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();

        //Set BoxCollider2D to sprite size
        Rect croppedRect = new Rect(
          (sprite.textureRectOffset.x + sprite.textureRect.width / 2f) / sprite.pixelsPerUnit,
          (sprite.textureRectOffset.y + sprite.textureRect.height / 2f) / sprite.pixelsPerUnit,
          sprite.textureRect.width / sprite.pixelsPerUnit,
          sprite.textureRect.height / sprite.pixelsPerUnit);

        // offset is relative to sprite's pivot
        collider.offset = croppedRect.position - sprite.pivot / sprite.pixelsPerUnit;
        collider.size = croppedRect.size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

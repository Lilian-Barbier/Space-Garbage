using Models;
using UnityEngine;
using Utils;

public class DominoBehavior : MonoBehaviour
{
    [SerializeField]
    private bool setRandomDomino = false;

    [SerializeField]
    private bool useDominoPieces = false;

    // get own SpriteRenderer
    SpriteRenderer spriteRenderer;
    GameManager gameManager;
    public Domino domino;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();

        
        
        
        if(setRandomDomino) {
          if(useDominoPieces)
            domino = new Domino(DominoUtils.GetRandomDominoPiece(), DominoUtils.GetRandomColor());
          else
            domino = new Domino(DominoUtils.GetRandomValidDomino(), DominoUtils.GetRandomColor());

        }

        SetSpriteAndCollider();
    }

    public void SetDomino(Domino domino)
    {
        this.domino = domino;
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

    //Fonctions utilis�s dans l'assembleur
    public void MoveDominoUp()
    {
        domino.Blocks = DominoUtils.MoveDominoUp(domino.Blocks);
    }
    public void MoveDominoRight()
    {
        domino.Blocks = DominoUtils.MoveDominoRight(domino.Blocks);
    }
    public void MoveDominoLeft()
    {
        domino.Blocks = DominoUtils.MoveDominoLeft(domino.Blocks);
    }
    public void MoveDominoDown()
    {
        domino.Blocks = DominoUtils.MoveDominoDown(domino.Blocks);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class DominoBehavior : MonoBehaviour
{
    // get own SpriteRenderer
    SpriteRenderer spriteRenderer;
    GameManager gameManager;

    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = gameManager.GenerateDominoSprite(new Domino(DominoManager.GetRandomValidDomino()));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using Models;
using UnityEngine;

public class HologramBehavior : MonoBehaviour
{
    Trash _domino;
    public Trash domino { get => _domino; }

    TrashGenerator dominoGenerator;

    private void Start()
    {
        dominoGenerator = FindObjectOfType<TrashGenerator>().GetComponent<TrashGenerator>();
    }


    public void SetDomino(Trash domino)
    {
        if (dominoGenerator == null)
            dominoGenerator = FindObjectOfType<TrashGenerator>().GetComponent<TrashGenerator>();

        this._domino = domino;
        var hologramSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        hologramSpriteRenderer.sprite = dominoGenerator.GenerateTrashSprite(domino);
    }
}

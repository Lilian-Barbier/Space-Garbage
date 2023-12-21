using UnityEngine;
using Models;
using Utils;

public class AssemblerBehavior : MonoBehaviour
{

    [SerializeField] GameObject hologramPrefab;
    [SerializeField] GameObject dominoPrefab;

    private TrashGenerator dominoGenerator;

    private SpriteRenderer dominoSpriteRenderer;

    private Trash currentDomino;

    void Start()
    {
        dominoGenerator = FindObjectOfType<TrashGenerator>().GetComponent<TrashGenerator>();

        dominoSpriteRenderer = GetComponentsInChildren<SpriteRenderer>()[1];

        InitDomino();
    }

    private void InitDomino()
    {
        currentDomino = new Trash(TrashUtils.None);
        dominoSpriteRenderer.sprite = dominoGenerator.GenerateTrashSprite(currentDomino);
    }

    public bool IsEmpty()
    {
        for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
                if (currentDomino.Blocks[i][j].Exists)
                    return false;

        return true;
    }

    public HologramBehavior AddHologram(Trash domino)
    {
        var hologram = Instantiate(hologramPrefab, transform);

        hologram.GetComponent<HologramBehavior>().SetDomino(domino);

        return hologram.GetComponent<HologramBehavior>();
    }

    public bool TryInsertHologram(HologramBehavior hologram)
    {
        if (CanAddHologram(hologram))
        {
            for (var i = 0; i < 4; i++)
                for (var j = 0; j < 4; j++)
                    if (!currentDomino.Blocks[i][j].Exists)
                        currentDomino.Blocks[i][j] = hologram.domino.Blocks[i][j];

            dominoSpriteRenderer.sprite = dominoGenerator.GenerateTrashSprite(currentDomino);
            return true;
        }

        return false;
    }

    public GameObject TakeDominoOut()
    {
        var newDomino = Instantiate(dominoPrefab);
        newDomino.transform.position = new Vector2(100, 100);
        currentDomino.isAssembled = true;


        newDomino.GetComponent<TrashBehaviour>().trash = currentDomino;

        InitDomino();
        return newDomino;
    }

    private bool CanAddHologram(HologramBehavior hologram)
    {

        for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
                if (currentDomino.Blocks[i][j].Exists && hologram.domino.Blocks[i][j].Exists)
                    return false;

        return true;
    }
}

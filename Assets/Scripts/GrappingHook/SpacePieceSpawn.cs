using System.Collections.Generic;
using Models;
using UnityEngine;
using Utils;

public class SpacePieceSpawn : MonoBehaviour
{
    [SerializeField] GameObject trashPrefab;

    [SerializeField] private float minTimeBetweenSpawns = 3f;
    [SerializeField] private float manTimeBetweenSpawns = 8f;

    [SerializeField] private float pieceSpeed = 1f;

    private BoxCollider2D boxCollider2D;
    public List<GameObject> piecesInstantiate = new List<GameObject>();
    private float timeBetweenSpawns;
    private float timeSinceLastSpawn = 0f;

    private SpaceshipManager spaceshipManager;

    void Start()
    {
        spaceshipManager = FindObjectOfType<SpaceshipManager>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        SetTimeBetweenSpawns();
    }

    private void SetTimeBetweenSpawns()
    {
        timeBetweenSpawns = Random.Range(minTimeBetweenSpawns, manTimeBetweenSpawns);
    }

    private void SpawnPieces()
    {
        var newTrash = Instantiate(trashPrefab, GetRandomPositionInBoxCollider(), Quaternion.identity);
        newTrash.GetComponent<Collider2D>().isTrigger = true;
        newTrash.GetComponent<TrashBehaviour>().trash = new Trash(TrashUtils.GetRandomValidDomino());
        piecesInstantiate.Add(newTrash);
    }

    private Vector3 GetRandomPositionInBoxCollider()
    {
        var randomX = Random.Range(boxCollider2D.bounds.min.x, boxCollider2D.bounds.max.x);
        var randomY = Random.Range(boxCollider2D.bounds.min.y, boxCollider2D.bounds.max.y);
        return new Vector3(randomX, randomY, 0);
    }

    void Update()
    {
        if (timeSinceLastSpawn >= timeBetweenSpawns)
        {
            SpawnPieces();
            SetTimeBetweenSpawns();
            timeSinceLastSpawn = 0f;
        }
        else
        {
            timeSinceLastSpawn += Time.deltaTime;
        }

        List<GameObject> toRemove = new List<GameObject>();
        foreach (var piece in piecesInstantiate)
        {
            piece.transform.position += pieceSpeed * spaceshipManager.speedLevel * Time.deltaTime * Vector3.down;

            if (piece.transform.position.y < -13)
            {
                toRemove.Add(piece);
            }
        }

        foreach (var piece in toRemove)
        {
            piecesInstantiate.Remove(piece);
            Destroy(piece);
        }
    }

    public GameObject RemovePiece(GameObject gameObject)
    {
        piecesInstantiate.Remove(gameObject);
        return gameObject;
    }
}

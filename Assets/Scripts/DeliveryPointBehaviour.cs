using Assets.Scripts.Enums;
using UnityEngine;

public class DeliveryPointBehaviour : TableBehaviour
{
    SpaceshipManager spaceshipManager;
    AudioSource audioSource;
    private void Start()
    {
        spaceshipManager = FindObjectOfType<SpaceshipManager>().GetComponent<SpaceshipManager>();
        audioSource = GetComponent<AudioSource>();
    }

    public override void SetObjectCarried(Transform newObjectCarried)
    {
        base.SetObjectCarried(newObjectCarried);

        TrashBehaviour trashBehaviour = newObjectCarried.GetComponent<TrashBehaviour>();

        var blocks = trashBehaviour.GetBlocksMaterialType();

        if (blocks.Count != 4 || blocks[0] != MaterialType.Metal || blocks[1] != MaterialType.Metal || blocks[2] != MaterialType.Metal || blocks[3] != MaterialType.Metal)
        {
            Debug.Log("Eject");
            EjectPiece(newObjectCarried);
        }
        else
        {
            spaceshipManager.money++;
            audioSource.Play();
            Destroy(newObjectCarried.gameObject);
        }
    }

    private void EjectPiece(Transform trash)
    {
        base.GetObjectCarried();
        trash.transform.position = transform.position + Vector3.right;
    }

}

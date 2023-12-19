using Assets.Scripts.Enums;
using UnityEngine;

public class DeliveryPointBehaviour : TableBehaviour
{
    SpaceshipManager spaceshipManager;
    private void Start()
    {
        spaceshipManager = FindObjectOfType<SpaceshipManager>().GetComponent<SpaceshipManager>();
    }

    public override void SetObjectCarried(Transform newObjectCarried)
    {
        TrashBehaviour trashBehaviour = newObjectCarried.GetComponent<TrashBehaviour>();

        if (!trashBehaviour.IsOnlyOneMaterialTrash(MaterialType.Metal) && trashBehaviour.GetTrashSize() != 4)
        {
            EjectPiece(newObjectCarried);
        }
        else
        {
            spaceshipManager.money++;
            Destroy(newObjectCarried.gameObject);
        }
    }

    private void EjectPiece(Transform trash)
    {
        trash.transform.position = transform.position + Vector3.right;
        trash.GetComponent<Collider2D>().isTrigger = false;
        trash.GetComponent<Rigidbody2D>().isKinematic = false;
    }
}

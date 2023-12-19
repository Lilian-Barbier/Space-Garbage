using Assets.Scripts.Enums;
using UnityEngine;

public class DeliveryPointFuel : TableBehaviour
{
    SpaceshipManager spaceshipManager;
    private void Start()
    {
        spaceshipManager = FindObjectOfType<SpaceshipManager>().GetComponent<SpaceshipManager>();
    }

    public override void SetObjectCarried(Transform newObjectCarried)
    {
        TrashBehaviour trashBehaviour = newObjectCarried.GetComponent<TrashBehaviour>();

        if (!trashBehaviour.IsOnlyOneMaterialTrash(MaterialType.Fuel))
        {
            EjectPiece(newObjectCarried);
        }
        else
        {
            spaceshipManager.DeliveryFuel(trashBehaviour.GetTrashSize());
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

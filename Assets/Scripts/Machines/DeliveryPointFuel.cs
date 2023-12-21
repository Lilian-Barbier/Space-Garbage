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
            newObjectCarried.GetComponent<Collider2D>().isTrigger = false;
            newObjectCarried.GetComponent<Rigidbody2D>().isKinematic = false;
            return;
        }

        spaceshipManager.DeliveryFuel(trashBehaviour.GetTrashSize());
        Destroy(newObjectCarried.gameObject);
    }

    public override Transform GetObjectCarried()
    {
        return null;
    }

}

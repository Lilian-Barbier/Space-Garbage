using Models;
using UnityEngine;
using Utils;

public class CrateBehaviour : MonoBehaviour
{
    [SerializeField] GameObject dominoPrefab;
    [SerializeField] int dominoLength = 1;
    [SerializeField] Vector2 releaseDirection = Vector2.up;

    public Transform GetObject(bool instantiateWithColliderTrigger = true)
    {
        //Dégueu
        var newDomino = Instantiate(dominoPrefab, new Vector3(100, 100, 0), Quaternion.identity);
        newDomino.GetComponent<Collider2D>().isTrigger = instantiateWithColliderTrigger;
        switch (dominoLength)
        {
            case 1:
                newDomino.GetComponent<TrashBehaviour>().trash = new Trash(TrashUtils.SingleBlockMetallic);
                break;
            case 2:
                newDomino.GetComponent<TrashBehaviour>().trash = new Trash(TrashUtils.DoubleBlockMetallic);
                break;
            default:
                newDomino.GetComponent<TrashBehaviour>().trash = new Trash(TrashUtils.TripleBlockMetallic);
                break;
        }

        return newDomino.transform;
        //Proc Animation
    }

    public void ReleaseDomino()
    {
        var block = GetObject(instantiateWithColliderTrigger: false);
        block.transform.position = transform.position + (Vector3)releaseDirection;
    }
}

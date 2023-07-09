using Models;
using UnityEngine;
using Utils;

public class BoxBehaviour : MonoBehaviour
{
    [SerializeField] GameObject dominoPrefab;
    [SerializeField] int dominoLength = 1;



    public Transform GetObject()
    {
        //Dégueu
        var newDomino = Instantiate(dominoPrefab, new Vector3(100, 100, 0), Quaternion.identity);
        newDomino.GetComponent<Collider2D>().isTrigger = true; 
        switch (dominoLength)
        {
            case 1:
                newDomino.GetComponent<DominoBehavior>().domino = new Domino(DominoUtils.SingleBlock);
                break;
            case 2:
                newDomino.GetComponent<DominoBehavior>().domino = new Domino(DominoUtils.DoubleBlock);
                break;
            default:
                newDomino.GetComponent<DominoBehavior>().domino = new Domino(DominoUtils.TripleBlock);
                break;
        }
        
        return newDomino.transform;
        //Proc Animation
    }
}

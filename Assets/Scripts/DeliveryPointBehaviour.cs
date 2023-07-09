using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class DeliveryPointBehaviour : MonoBehaviour
{
    public List<Domino> distribuedDomino = new List<Domino>();
 
    GameManager gameManager;
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
    }

    public void DeliveryDomino(Domino domino)
    {
        distribuedDomino.Add(domino);
        for (var i = gameManager.dominoRequestList.Count - 1; i > -1; i--)
        {
            var dominoReq = gameManager.dominoRequestList[i];
            var res = DominoUtils.isDominoFullfillingRequest(domino, dominoReq);

            if (res)
            {
                gameManager.DeleteDominoRequest(i);
                gameManager.GainScore(dominoReq.RemainingTime/dominoReq.InitialDuration);
                return;
            }
        }

        gameManager.LooseScore();
    }
}

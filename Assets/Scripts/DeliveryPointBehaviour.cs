using Models;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class DeliveryPointBehaviour : MonoBehaviour
{

    GameManager gameManager;
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
    }

    public void DeliveryDomino(Domino domino)
    {
        for (var i = gameManager.dominoRequestList.Count - 1; i > -1; i--)
        {
            var res = DominoUtils.isDominoFullfillingRequest(domino, gameManager.dominoRequestList[i]);

            if (res)
            {
                gameManager.DeleteDominoRequest(i);
                gameManager.GainScore();
                return;
            }
        }

        gameManager.LooseScore();
    }
}

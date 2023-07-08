using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerBehavior : MonoBehaviour
{
    private Image timerFill;
    
    private int requestNumber;

    private GameManager gameManager;
    void Start()
    {
        requestNumber = 0;
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        
        timerFill = transform.Find("Timer").GetComponent<Image>();

        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        while (gameManager.dominoRequestTimeList.Count == 0)
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        while (gameManager.dominoRequestTimeList[requestNumber] >= 0f)
        {
            timerFill.fillAmount = gameManager.dominoRequestTimeList[requestNumber] / gameManager.dominoRequestDuration;
            yield return new WaitForSeconds(1/60f);
        }
    }
}

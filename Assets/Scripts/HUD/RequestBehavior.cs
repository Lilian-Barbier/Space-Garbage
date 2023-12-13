using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Utils;
using TMPro;

public class RequestBehavior : MonoBehaviour
{
  private Image timerFill;
  private TMP_Text playerText;
  private Image dominoImage;
  
  private DominoRequest dominoRequest;

  private TrashGenerator dominoGenerator;
  void Start()
  {
    dominoGenerator = FindObjectOfType<TrashGenerator>().GetComponent<TrashGenerator>();
    
    timerFill = transform.Find("Timer").GetComponent<Image>();
    playerText = transform.Find("PlayerInfos").GetComponent<TextMeshProUGUI>();
    dominoImage = transform.Find("Domino").GetComponent<Image>();

    //StartCoroutine(UpdateTimer());
  }

  public void SetDominoRequest(DominoRequest request)
  {
    dominoRequest = request;
  }

  //private IEnumerator UpdateTimer()
  //{
  //  while (dominoRequest == null)
  //    yield return new WaitForSeconds(1/60f);

  //  playerText.text = dominoRequest.Player.Name + ", " + dominoRequest.Player.Age + " yo";

  //  var domino = new Trash(dominoRequest.Blocks, dominoRequest.Color);
  //  var dominoSprite = dominoGenerator.GenerateTrashSprite(domino);
  //  dominoImage.sprite = dominoSprite;

  //  while (dominoRequest.RemainingTime >= 0f)
  //  {
  //    timerFill.fillAmount = dominoRequest.RemainingTime / dominoRequest.InitialDuration;
  //    yield return new WaitForSeconds(1/60f);
  //  }
  //}
}

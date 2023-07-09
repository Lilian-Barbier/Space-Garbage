using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HearthManager : MonoBehaviour
{
    Image[] hearths;
    private TMP_Text scoreText;

    private int currentScore = 1;


    // Start is called before the first frame update
    void Start()
    {
        hearths = GetComponentsInChildren<Image>();
        scoreText = GetComponentInChildren<TMP_Text>();
    }

    public void LifeChanged(int life)
    {
        for(int i = 0; i < hearths.Length; i++)
        {
            hearths[i].enabled = i < life;
        }
    }

    public void ScoreChanged(int score)
    {
      if(currentScore == null || score < currentScore) {
        scoreText.text = score.ToString();
        currentScore = score;
      }
      else {
        StartCoroutine(UpdateScore(score));
        StartCoroutine(PulseScoreText());
      }
    }

    private IEnumerator UpdateScore(int score)
    {
      var scoreBeforeUpdate = currentScore;
      currentScore = score;
      
      int difference = score - scoreBeforeUpdate;

      for(int i = 0; i < difference; i+=2)
      {
        scoreText.text = (scoreBeforeUpdate + i).ToString();
        yield return new WaitForSeconds(0.03f);
      }
      scoreText.text = score.ToString();
    }

    private IEnumerator PulseScoreText()
    {
      for (float i = 1f; i <= 1.1f; i += 0.01f) {
        scoreText.rectTransform.localScale = new Vector3(i, i, i);
        yield return new WaitForSeconds(0.02f);
      }

      scoreText.rectTransform.localScale = new Vector3(1.1f, 1.1f, 1.1f);

      for (float i = 1.1f; i >= 1f; i -= 0.01f) {
        scoreText.rectTransform.localScale = new Vector3(i, i, i);
        yield return new WaitForSeconds(0.02f);
      }

      scoreText.rectTransform.localScale = new Vector3(1f, 1f, 1f);
    } 

    public int GetCurrentScore() {
      return currentScore;
    }
}

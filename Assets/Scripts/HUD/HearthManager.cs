using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HearthManager : MonoBehaviour
{
    Image[] hearths;
    private TMP_Text scoreText;


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
        scoreText.text = score.ToString();
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenuManager : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<CanvasGroup>().alpha = 0;
    }

    public void EndLevel(int score)
    {
        GetComponent<CanvasGroup>().alpha = 1;
        transform.Find("FinalScore").GetComponent<TextMeshProUGUI>().text = "SCORE : " + score;

    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("ChooseLvl");
    }
}

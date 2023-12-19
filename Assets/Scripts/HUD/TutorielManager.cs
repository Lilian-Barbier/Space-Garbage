using UnityEngine;

public class TutorielManager : Singleton<TutorielManager>
{
    [SerializeField] private CanvasGroup[] tutorials;
    private int currentTutorial = 0;

    public bool tutorialHookPassed = false;
    public bool tutorialDividerPassed = false;
    public bool tutorialMergerPassed = false;
    public bool tutorialComputerPassed = false;
    public bool tutorialCarburatorPassed = false;
    public bool tutorialStartPassed = false;

    public void NextTutorial()
    {
        if (currentTutorial < tutorials.Length - 1)
        {
            tutorials[currentTutorial].alpha = 0;
            currentTutorial++;
            tutorials[currentTutorial].alpha = 1;
        }
        else
        {
            Debug.Log("End of tutorial");
            tutorials[currentTutorial].alpha = 0;
            GetComponent<CanvasGroup>().alpha = 0;
        }
    }
}

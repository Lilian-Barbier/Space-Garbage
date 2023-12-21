using UnityEngine;
using UnityEngine.InputSystem;

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

    private bool tutorialPassed = false;

    public void NextTutorial()
    {
        if (currentTutorial < tutorials.Length - 1)
        {
            tutorials[currentTutorial].alpha = 0;
            currentTutorial++;
            tutorials[currentTutorial].alpha = 1;
        }
        else if (!tutorialPassed)
        {
            Debug.Log("End of tutorial");
            tutorials[currentTutorial].alpha = 0;
            GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    public void PassTutorial(InputAction.CallbackContext ctx)
    {
        if (!ctx.started || tutorialPassed) return;

        tutorialPassed = true;
        tutorials[currentTutorial].alpha = 0;
        GetComponent<CanvasGroup>().alpha = 0;
        currentTutorial = tutorials.Length;
    }
}

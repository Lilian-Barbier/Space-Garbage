using UnityEngine;

public class ComputerButton : MonoBehaviour
{
    [SerializeField] ChooseComputerUpgrade chooseComputerUpgrade;

    public void ShowComputer()
    {
        chooseComputerUpgrade.ShowCanvas();
    }

    public void HideComputer()
    {
        chooseComputerUpgrade.HideCanvas();
    }
}

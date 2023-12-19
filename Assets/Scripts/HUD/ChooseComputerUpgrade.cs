using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseComputerUpgrade : MonoBehaviour
{
    [Serializable]
    private class UpgradeInformation
    {
        [SerializeField] public Image image;
        [SerializeField] public CanvasGroup informationGroup;
        [SerializeField] public int price;
        [SerializeField] public Upgrade upgradeType;
        [SerializeField] public int limitedUse = -1;
    }

    [SerializeField] List<UpgradeInformation> upgrades;
    CharacterController characterController;
    HookController hookController;
    SpaceshipManager spaceshipManager;

    public enum Upgrade
    {
        CarburatorEngine,
        FilterEngine,
        MergerEngine,
        DividerEngine,
        ConveyorBelt,
        SpaceshipSpeed,
        GrappingHookSpeed,
        GrappingHookRange,
        AutomaticGrappingHook
    }

    int selectedImage = 0;

    CanvasGroup computerCanvas;

    void Start()
    {
        characterController = FindObjectOfType<CharacterController>();
        hookController = FindObjectOfType<HookController>();
        spaceshipManager = FindObjectOfType<SpaceshipManager>();

        computerCanvas = GetComponent<CanvasGroup>();
        computerCanvas.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            if (i == selectedImage)
            {
                upgrades[i].image.color = new Color(1, 1, 1, 1f);
                upgrades[i].informationGroup.alpha = 1;
            }
            else
            {
                upgrades[i].image.color = new Color(1, 1, 1, 0.5f);
                upgrades[i].informationGroup.alpha = 0;
            }
        }
    }

    public void SelectNext()
    {
        upgrades[selectedImage].image.color = new Color(1, 1, 1, 0.5f);
        upgrades[selectedImage].informationGroup.alpha = 0;
        selectedImage++;
        if (selectedImage >= upgrades.Count)
        {
            selectedImage = 0;
        }
        upgrades[selectedImage].image.color = new Color(1, 1, 1, 1f);
        upgrades[selectedImage].informationGroup.alpha = 1;
    }

    public void SelectPrevious()
    {
        upgrades[selectedImage].image.color = new Color(1, 1, 1, 0.5f);
        upgrades[selectedImage].informationGroup.alpha = 0;
        selectedImage--;
        if (selectedImage < 0)
        {
            selectedImage = upgrades.Count - 1;
        }
        upgrades[selectedImage].image.color = new Color(1, 1, 1, 1f);
        upgrades[selectedImage].informationGroup.alpha = 1;
    }

    public int Buy(int currentMoney)
    {
        if (currentMoney >= upgrades[selectedImage].price)
        {
            switch (upgrades[selectedImage].upgradeType)
            {
                case Upgrade.CarburatorEngine:
                    characterController.nbOfFuelMachine += 1;
                    if (!TutorielManager.Instance.tutorialComputerPassed)
                    {
                        TutorielManager.Instance.tutorialComputerPassed = true;
                        TutorielManager.Instance.NextTutorial();
                    }
                    break;
                case Upgrade.FilterEngine:
                    characterController.nbOfFilter += 1;
                    break;
                case Upgrade.MergerEngine:
                    characterController.nbOfFusionMachine += 1;
                    break;
                case Upgrade.DividerEngine:
                    characterController.nbOfSeparator += 1;
                    break;
                case Upgrade.ConveyorBelt:
                    characterController.nbOfConveyorBelt += 8;
                    break;
                case Upgrade.SpaceshipSpeed:
                    spaceshipManager.speedLevel += 1;
                    break;
                case Upgrade.GrappingHookSpeed:
                    hookController.speed += 1;
                    break;
                case Upgrade.GrappingHookRange:
                    hookController.maxDistance += 1;
                    break;
                case Upgrade.AutomaticGrappingHook:
                    hookController.automaticHook = true;
                    break;
            }
            if (upgrades[selectedImage].limitedUse != -1)
            {
                upgrades[selectedImage].limitedUse -= 1;
                if (upgrades[selectedImage].limitedUse <= 0)
                {
                    upgrades[selectedImage].image.color = new Color(1, 1, 1, 0f);
                    upgrades.RemoveAt(selectedImage);
                    selectedImage = 0;
                }
            }
            return currentMoney - upgrades[selectedImage].price;
        }
        else
        {
            return currentMoney;
        }
    }

    public void ShowCanvas()
    {
        computerCanvas.alpha = 1;
    }

    public void HideCanvas()
    {
        computerCanvas.alpha = 0;
    }

}

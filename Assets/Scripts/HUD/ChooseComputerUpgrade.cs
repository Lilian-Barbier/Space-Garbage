using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        [SerializeField] public int unite;
        [SerializeField] public Upgrade upgradeType;
        [SerializeField] public int limitedUse = -1;
    }

    [SerializeField] List<UpgradeInformation> upgrades;
    CharacterController characterController;
    HookController hookController;
    SpaceshipManager spaceshipManager;

    [SerializeField] private TextMeshProUGUI limitedUseText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI uniteText;

    private Color defaultColor = new Color(0.8679245f, 0.8679245f, 0.8679245f, 0.3f);
    private Color selectColor = new Color(0.8679245f, 0.8679245f, 0.8679245f, 1f);
    private Color buyColor = new Color(1, 0.8638987f, 0.5037736f, 1f);

    AudioSource audioSource;

    public enum Upgrade
    {
        CarburatorEngine,
        FilterEngine,
        MergerEngine,
        DividerEngine,
        ConveyorBelt,
        SpaceshipSpeed,
        GrappingHookSpeed,
        GrappingHookReloadSpeed,
        AutomaticGrappingHook
    }

    int selectedImage = 0;

    CanvasGroup computerCanvas;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        characterController = FindObjectOfType<CharacterController>();
        hookController = FindObjectOfType<HookController>();
        spaceshipManager = FindObjectOfType<SpaceshipManager>();

        computerCanvas = GetComponent<CanvasGroup>();
        computerCanvas.alpha = 0;

        for (int i = 0; i < upgrades.Count; i++)
        {
            if (i == selectedImage)
            {
                upgrades[i].image.color = selectColor;
                upgrades[i].informationGroup.alpha = 1;
            }
            else
            {
                upgrades[i].image.color = defaultColor;
                upgrades[i].informationGroup.alpha = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectNext()
    {
        upgrades[selectedImage].image.color = defaultColor;
        upgrades[selectedImage].informationGroup.alpha = 0;
        selectedImage++;
        if (selectedImage >= upgrades.Count)
        {
            selectedImage = 0;
        }
        upgrades[selectedImage].image.color = selectColor;
        upgrades[selectedImage].informationGroup.alpha = 1;

        priceText.text = upgrades[selectedImage].price.ToString();
        uniteText.text = upgrades[selectedImage].unite.ToString();

        if (upgrades[selectedImage].limitedUse != -1)
        {
            limitedUseText.text = upgrades[selectedImage].limitedUse.ToString();
        }
        else
        {
            limitedUseText.text = "";
        }
    }

    public void SelectPrevious()
    {
        upgrades[selectedImage].image.color = defaultColor;
        upgrades[selectedImage].informationGroup.alpha = 0;
        selectedImage--;
        if (selectedImage < 0)
        {
            selectedImage = upgrades.Count - 1;
        }
        upgrades[selectedImage].image.color = selectColor;
        upgrades[selectedImage].informationGroup.alpha = 1;

        priceText.text = upgrades[selectedImage].price.ToString();
        uniteText.text = upgrades[selectedImage].unite.ToString();

        if (upgrades[selectedImage].limitedUse != -1)
        {
            limitedUseText.text = upgrades[selectedImage].limitedUse.ToString();
        }
        else
        {
            limitedUseText.text = "";
        }
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
                    hookController.speed += 2;
                    break;
                case Upgrade.GrappingHookReloadSpeed:
                    if (hookController.timeBetweenLaunch > 0.4f)
                        hookController.timeBetweenLaunch -= 0.4f;
                    break;
                case Upgrade.AutomaticGrappingHook:
                    hookController.automaticHook = true;
                    break;
            }

            upgrades[selectedImage].image.color = buyColor;
            StartCoroutine(RestoreColor(selectedImage));
            audioSource.Play();

            if (upgrades[selectedImage].limitedUse != -1)
            {
                upgrades[selectedImage].limitedUse -= 1;
                if (upgrades[selectedImage].limitedUse <= 0)
                {
                    upgrades[selectedImage].informationGroup.alpha = 0;
                    upgrades[selectedImage].image.color = new Color(1, 1, 1, 0f);
                    upgrades.RemoveAt(selectedImage);
                    selectedImage = 1;
                    SelectPrevious();
                }
                else
                {
                    limitedUseText.text = upgrades[selectedImage].limitedUse.ToString();
                }
            }


            return currentMoney - upgrades[selectedImage].price;
        }
        else
        {
            return currentMoney;
        }
    }

    private IEnumerator RestoreColor(int index)
    {
        yield return new WaitForSeconds(0.2f);
        if (index < upgrades.Count && upgrades[index].limitedUse == -1 || upgrades[index].limitedUse > 0)
            upgrades[index].image.color = selectedImage == index ? selectColor : defaultColor;
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

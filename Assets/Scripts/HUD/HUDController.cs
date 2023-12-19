using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CharacterController;

public class HUDController : MonoBehaviour
{

    [SerializeField] Image FusionImage;
    [SerializeField] Image FilterImage;
    [SerializeField] Image SeparatorImage;
    [SerializeField] Image CarburatorImage;
    [SerializeField] Image HandImage;
    [SerializeField] Image EraseImage;
    [SerializeField] Image ConveyorImage;

    TextMeshProUGUI FusionCount;
    TextMeshProUGUI FilterCount;
    TextMeshProUGUI SeparatorCount;
    TextMeshProUGUI CarburatorCount;
    TextMeshProUGUI ConveyorCount;

    CharacterController characterController;

    void Start()
    {
        characterController = GameObject.FindWithTag("Player").GetComponent<CharacterController>();
        FusionCount = FusionImage.GetComponentInChildren<TextMeshProUGUI>();
        FilterCount = FilterImage.GetComponentInChildren<TextMeshProUGUI>();
        SeparatorCount = SeparatorImage.GetComponentInChildren<TextMeshProUGUI>();
        CarburatorCount = CarburatorImage.GetComponentInChildren<TextMeshProUGUI>();
        ConveyorCount = ConveyorImage.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        FusionCount.text = characterController.nbOfFusionMachine.ToString();
        FilterCount.text = characterController.nbOfFilter.ToString();
        SeparatorCount.text = characterController.nbOfSeparator.ToString();
        CarburatorCount.text = characterController.nbOfFuelMachine.ToString();
        ConveyorCount.text = characterController.nbOfConveyorBelt.ToString();

        if (characterController.selectedAction == PlayerAction.ConstructFusionMachine)
        {
            FusionImage.color = new Color(1, 1, 1, 1);
        }
        else
        {
            FusionImage.color = new Color(1, 1, 1, 0.2f);
        }

        if (characterController.selectedAction == PlayerAction.ConstructFilter)
        {
            FilterImage.color = new Color(1, 1, 1, 1);
        }
        else
        {
            FilterImage.color = new Color(1, 1, 1, 0.2f);
        }

        if (characterController.selectedAction == PlayerAction.ConstructSeparator)
        {
            SeparatorImage.color = new Color(1, 1, 1, 1);
        }
        else
        {
            SeparatorImage.color = new Color(1, 1, 1, 0.2f);
        }

        if (characterController.selectedAction == PlayerAction.ConstructFuelMachine)
        {
            CarburatorImage.color = new Color(1, 1, 1, 1);
        }
        else
        {
            CarburatorImage.color = new Color(1, 1, 1, 0.2f);
        }

        if (characterController.selectedAction == PlayerAction.TakeObject)
        {
            HandImage.color = new Color(1, 1, 1, 1);
        }
        else
        {
            HandImage.color = new Color(1, 1, 1, 0.2f);
        }

        if (characterController.selectedAction == PlayerAction.RemoveConstruction)
        {
            EraseImage.color = new Color(1, 1, 1, 1);
        }
        else
        {
            EraseImage.color = new Color(1, 1, 1, 0.2f);
        }

        if (characterController.selectedAction == PlayerAction.ConstructConveyor)
        {
            ConveyorImage.color = new Color(1, 1, 1, 1);
        }
        else
        {
            ConveyorImage.color = new Color(1, 1, 1, 0.2f);
        }
    }
}

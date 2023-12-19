using System.Collections;
using TMPro;
using UnityEngine;

public class SpaceshipManager : MonoBehaviour
{

    private int _money = 0;
    public int money
    {
        get => _money;
        set
        {
            _money = value;
            moneyText.text = _money.ToString() + " #";
        }
    }

    [SerializeField] TextMeshProUGUI moneyText;


    int currentFuel = 0;
    [SerializeField] TextMeshProUGUI fuelText;

    int distance = 0;
    [SerializeField] TextMeshProUGUI distanceText;


    public ChooseComputerUpgrade chooseComputerUpgrade;

    public int speedLevel = 1;

    private float timeFor1FuelSpeedLevel1 = 10f;
    private float timeFor1FuelSpeedLevel2 = 7f;
    private float timeFor1FuelSpeedLevel3 = 4f;

    private float currentFuelConsumption;
    private float lastTimeFuelConsumption = 0f;

    private float timeFor1Meter = 0.3f;
    private float lastTimeDistance = 0f;

    private bool isMoving = false;

    Material spaceBackgroundMaterial;
    [SerializeField] GameObject background;

    private void Start()
    {
        spaceBackgroundMaterial = background.GetComponent<SpriteRenderer>().material;
    }

    void Update()
    {
        lastTimeDistance += Time.deltaTime;

        if (isMoving && lastTimeDistance > timeFor1Meter)
        {
            lastTimeDistance = 0f;
            distance += speedLevel * 2;
            distanceText.text = distance.ToString();
        }
    }

    private float GetFuelConsumption()
    {
        return speedLevel switch
        {
            1 => timeFor1FuelSpeedLevel1,
            2 => timeFor1FuelSpeedLevel2,
            3 => timeFor1FuelSpeedLevel3,
            _ => timeFor1FuelSpeedLevel1,
        };
    }

    internal void DeliveryFuel(int fuel)
    {
        if (!TutorielManager.Instance.tutorialStartPassed)
        {
            Debug.Log("Finish");
            TutorielManager.Instance.tutorialStartPassed = true;
            TutorielManager.Instance.NextTutorial();
        }

        currentFuel += fuel;
        fuelText.text = currentFuel.ToString();

        if (!isMoving)
            StartCoroutine(nameof(ConsumeFuel));
    }

    IEnumerator ConsumeFuel()
    {
        isMoving = true;
        spaceBackgroundMaterial.SetFloat("_Speed", 2 * speedLevel);
        currentFuel--;
        fuelText.text = currentFuel.ToString();
        yield return new WaitForSeconds(GetFuelConsumption());
        if (currentFuel > 0)
        {
            StartCoroutine(nameof(ConsumeFuel));
        }
        else
        {
            isMoving = false;
            spaceBackgroundMaterial.SetFloat("_Speed", 0);
        }
    }

    //Computer Manager

    public void ShowComputer()
    {
        chooseComputerUpgrade.ShowCanvas();
    }

    public void HideComputer()
    {
        chooseComputerUpgrade.HideCanvas();
    }

    public void SelectNextComputer()
    {
        chooseComputerUpgrade.SelectNext();
    }

    public void SelectPreviousComputer()
    {
        chooseComputerUpgrade.SelectPrevious();
    }

    public void BuyUpgrade()
    {
        money = chooseComputerUpgrade.Buy(money);
    }
}

using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceshipManager : MonoBehaviour
{
    //Todo à implémenter
    public float ConveyorSpeed = 1f;
    public int CurrentElectricity = 50;


    private int _money = 0;
    public int money
    {
        get => _money;
        set
        {
            _money = value;
            moneyText.text = _money.ToString();
        }
    }

    [SerializeField] TextMeshProUGUI moneyText;


    int currentFuel = 0;
    [SerializeField] TextMeshProUGUI fuelText;

    int distance = 0;
    [SerializeField] TextMeshProUGUI distanceText;

    float currentTime = 0;
    [SerializeField] TextMeshProUGUI timerText;


    [SerializeField] CanvasGroup scoreGroup;
    [SerializeField] TextMeshProUGUI scoreText;


    public ChooseComputerUpgrade chooseComputerUpgrade;

    public int speedLevel = 1;

    private float timeFor1FuelSpeedLevel1 = 5f;
    private float timeFor1FuelSpeedLevel2 = 3f;
    private float timeFor1FuelSpeedLevel3 = 2f;
    private float timeFor1FuelSpeedLevel4 = 1f;

    private float currentFuelConsumption;
    private float lastTimeFuelConsumption = 0f;

    private float timeFor1Meter = 0.3f;
    private float lastTimeDistance = 0f;

    private bool isMoving = false;

    Material spaceBackgroundMaterial;
    [SerializeField] GameObject background;

    AudioSource audioSource;

    private bool scoreShown = false;
    private void Start()
    {
        spaceBackgroundMaterial = background.GetComponent<SpriteRenderer>().material;
        audioSource = GetComponent<AudioSource>();
        money = 100;
        DeliveryFuel(50);
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        string formatTime = MathF.Floor(currentTime / 60).ToString("00") + ":" + (currentTime % 60).ToString("00");
        timerText.text = formatTime;

        //si le temps est supérieur à 20 minutes, on affiche le score
        if (currentTime > 1200 && !scoreShown)
        {
            Debug.Log("ScoreFinish");
            scoreShown = true;
            scoreGroup.alpha = 1;
            scoreText.text = distance.ToString();
        }

        lastTimeDistance += Time.deltaTime;

        if (isMoving && lastTimeDistance > timeFor1Meter)
        {
            lastTimeDistance = 0f;
            distance += speedLevel * (int)Math.Floor(Mathf.Sqrt(speedLevel));
            distanceText.text = distance.ToString();
        }
    }

    public void RemoveScore(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;

        if (scoreShown)
        {
            scoreGroup.alpha = 0;
        }
    }
    public void PlayStopMusic(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        else
        {
            audioSource.Play();
        }
    }

    private float GetFuelConsumption()
    {
        return speedLevel switch
        {
            1 => timeFor1FuelSpeedLevel1,
            2 => timeFor1FuelSpeedLevel2,
            3 => timeFor1FuelSpeedLevel3,
            _ => timeFor1FuelSpeedLevel4,
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
        spaceBackgroundMaterial.SetFloat("_Speed", speedLevel * Mathf.Sqrt(speedLevel));
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

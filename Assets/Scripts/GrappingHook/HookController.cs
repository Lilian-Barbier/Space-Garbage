using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HookController : MonoBehaviour
{
    [SerializeField] public float speed = 3f;
    [SerializeField] public float timeBetweenLaunch = 2f;
    private float timeSinceLastLaunch = 0f;

    [SerializeField] SpacePieceSpawn spacePieceSpawn;
    [SerializeField] GameObject laser;

    private Slider slider;

    //Angle from Vector2.left
    float angleDirection = 0;
    Vector2 dispenserPosition;
    Vector2 initialPosition;

    public bool automaticHook = true;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        dispenserPosition = transform.GetSiblingIndex() < transform.parent.childCount - 1 ? transform.parent.GetChild(transform.GetSiblingIndex() + 1).position : Vector2.zero;
        slider = GetComponentInChildren<Slider>();
        slider.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (automaticHook)
        {
            audioSource.volume = 0.08f;
        }

        timeSinceLastLaunch += Time.deltaTime;

        slider.value = timeSinceLastLaunch / timeBetweenLaunch;

        if (timeSinceLastLaunch > timeBetweenLaunch)
        {
            slider.gameObject.SetActive(false);
        }

        if (automaticHook && spacePieceSpawn.piecesInstantiate.Count > 0)
        {
            var piece = spacePieceSpawn.piecesInstantiate.OrderBy(p => Vector2.Distance(p.transform.position, transform.position)).FirstOrDefault();

            if (piece != null)
            {
                var direction = piece.transform.position - transform.position;
                var angle = Vector2.SignedAngle(Vector2.left, direction);
                var angleDifference = angle - angleDirection;

                if (angleDifference > 3)
                {
                    UpHook();
                }
                else if (angleDifference < -3)
                {
                    DownHook();
                }
                else
                {
                    LaunchHook();
                }
            }
        }
    }

    public void LaunchHook()
    {
        if (timeSinceLastLaunch > timeBetweenLaunch)
        {
            timeSinceLastLaunch = 0f;
            slider.gameObject.SetActive(true);

            var laser = Instantiate(this.laser, transform.position, transform.rotation);
            Laser laserComponent = laser.GetComponent<Laser>();
            laserComponent.dispenserPosition = dispenserPosition;
            laserComponent.spacePieceSpawn = spacePieceSpawn;
            laserComponent.speed = speed;

            audioSource.Play();
        }
    }

    public void UpHook()
    {
        //rotate direction of 5°
        angleDirection += 3;
        transform.Rotate(0, 0, 3);
    }

    public void DownHook()
    {
        //rotate direction of 5°
        angleDirection -= 3;
        transform.Rotate(0, 0, -3);
    }

}

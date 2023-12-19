using System.Linq;
using UnityEngine;

public class HookController : MonoBehaviour
{
    [SerializeField] public float speed = 1f;
    [SerializeField] public float maxDistance = 5f;
    [SerializeField] SpacePieceSpawn spacePieceSpawn;

    //Angle from Vector2.left
    float angleDirection = 0;
    Vector2 dispenserPosition;
    Vector2 initialPosition;

    bool hookLaunch = false;
    bool hookReturn = false;

    public bool automaticHook = true;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        dispenserPosition = transform.GetSiblingIndex() < transform.parent.childCount - 1 ? transform.parent.GetChild(transform.GetSiblingIndex() + 1).position : Vector2.zero;
    }

    void Update()
    {
        if (automaticHook && spacePieceSpawn.piecesInstantiate.Count > 0)
        {
            var piece = spacePieceSpawn.piecesInstantiate.OrderBy(p => Vector2.Distance(p.transform.position, transform.position)).FirstOrDefault();

            if (piece != null && !hookLaunch && !hookReturn)
            {
                var direction = piece.transform.position - transform.position;
                var angle = Vector2.SignedAngle(Vector2.left, direction);
                var angleDifference = angle - angleDirection;

                if (angleDifference > 5)
                {
                    UpHook();
                }
                else if (angleDifference < -5)
                {
                    DownHook();
                }
                else
                {
                    LaunchHook();
                }
            }
        }

        if (hookLaunch)
        {
            transform.position = Vector2.MoveTowards(transform.position, transform.position - transform.right, speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, initialPosition) > maxDistance)
            {
                hookLaunch = false;
                hookReturn = true;
            }
        }
        else if (hookReturn)
        {
            transform.position = Vector2.MoveTowards(transform.position, transform.position + transform.right, speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, initialPosition) < 0.1f)
            {
                hookReturn = false;
            }
        }

    }

    public void LaunchHook()
    {
        if (!hookLaunch && !hookReturn)
        {
            hookLaunch = true;
        }
    }

    public void ReturnHook()
    {
        if (hookLaunch)
        {
            hookLaunch = false;
            hookReturn = true;
        }
    }

    public void UpHook()
    {
        if (!hookLaunch && !hookReturn)
        {
            //rotate direction of 5°
            angleDirection += 5;
            transform.Rotate(0, 0, 5);
        }
    }

    public void DownHook()
    {
        if (!hookLaunch && !hookReturn)
        {
            //rotate direction of 5°
            angleDirection -= 5;
            transform.Rotate(0, 0, -5);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Blocks"))
        {
            if (!TutorielManager.Instance.tutorialHookPassed)
            {
                TutorielManager.Instance.tutorialHookPassed = true;
                TutorielManager.Instance.NextTutorial();
            }

            other.gameObject.transform.position = dispenserPosition;
            other.gameObject.GetComponent<Collider2D>().isTrigger = false;
            spacePieceSpawn.RemovePiece(other.gameObject);
        }
    }
}

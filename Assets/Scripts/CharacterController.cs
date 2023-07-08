using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Windows;

public class CharacterController : MonoBehaviour
{
    private PlayerInput playerInput;
    private bool isStopped;
    private Rigidbody2D rigidbody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Transform objectCarried;
    private Transform interactTriggerZone;
    private float lastDirectionAngle;

    [SerializeField]
    private float objectCarriedDistanceFactor = 0.7f;

    [SerializeField]
    private float speed;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Redomino.Action.performed += ctx => Interact();
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        interactTriggerZone = transform.GetChild(0);
    }
    private void OnEnable()
    {
        playerInput.Redomino.Enable();
    }

    private void OnDisable()
    {
        playerInput.Redomino.Disable();
    }


    private void FixedUpdate()
    {
        Vector2 movement = playerInput.Redomino.Movement.ReadValue<Vector2>();

        var isMovingSide = false;
        var isMovingUp = false;
        var isMovingDown = false;

        if (!isStopped)
        {
            if (movement.x != 0)
            {
                isMovingSide = true;
                spriteRenderer.flipX = movement.x < 0;
                lastDirectionAngle = movement.x > 0 ? 90 : -90 ;
            }
            else if (movement.y < 0)
            {
                isMovingDown = true;
                lastDirectionAngle = 0;
            }
            else if (movement.y > 0)
            {
                isMovingUp = true;
                lastDirectionAngle = 180;
            }

            interactTriggerZone.rotation = Quaternion.Euler(0, 0, lastDirectionAngle);
            rigidbody.MovePosition(rigidbody.position + movement * speed * Time.deltaTime);
        }

        animator.SetBool("isMovingSide", isMovingSide);
        animator.SetBool("isMovingUp", isMovingUp);
        animator.SetBool("isMovingDown", isMovingDown);

        if(objectCarried != null)
        {
            if(movement != Vector2.zero)
            {
                objectCarried.position = transform.position + (Vector3)movement.normalized * objectCarriedDistanceFactor;
                objectCarried.GetComponent<SpriteRenderer>().sortingOrder = movement.y < 0 && movement.x < 0.4 && movement.x > -0.4 ? 15 : 5;
            }
            else
            {
                objectCarried.position = transform.position + Vector3.down * objectCarriedDistanceFactor;
                objectCarried.GetComponent<SpriteRenderer>().sortingOrder = 15;
            }
           
        }
    }

    void Interact()
    {
        if(objectCarried != null)
        {
            DropObject();
        }
        else
        {
            GetObjectNear();
        }
    }

    private void GetObjectNear()
    {
        CapsuleCollider2D c = new CapsuleCollider2D();

        var capsuleTriggerZone = interactTriggerZone.GetComponent<CapsuleCollider2D>();

        List<Collider2D> interactableObjects = new List<Collider2D>();
        Physics2D.OverlapCollider(capsuleTriggerZone, new ContactFilter2D(), interactableObjects);

        //D'abord on vérifie si on peut prendre un bloc devant nous
        if (interactableObjects.Any(o => o.transform.CompareTag("Blocks")))
        {
            var collider = interactableObjects.First(o => o.transform.CompareTag("Blocks"));
            collider.isTrigger = true;
            objectCarried = collider.transform;

            return;
        }

        //Sinon on prend un bloc d'une table
        if (interactableObjects.Any(o => o.transform.CompareTag("Table"))){
            objectCarried = interactableObjects.First(o => o.transform.CompareTag("Table")).GetComponent<TableBehaviour>().GetObjectCarried();
            return;
        }

        //Dans le dernier cas on vérifie si on ne peux pas récupérer un bloc proche autour du joueur
        var circleTriggerZone = interactTriggerZone.GetComponent<CircleCollider2D>();
        interactableObjects = new List<Collider2D>();
        Physics2D.OverlapCollider(circleTriggerZone, new ContactFilter2D(), interactableObjects);

        if (interactableObjects.Any(o => o.transform.CompareTag("Blocks")))
        {
            var collider = interactableObjects.First(o => o.transform.CompareTag("Blocks"));
            collider.isTrigger = true;
            objectCarried = collider.transform;
        }

    }

    private void DropObject()
    {
        CapsuleCollider2D c = new CapsuleCollider2D();

        var capsuleTriggerZone = interactTriggerZone.GetComponent<CapsuleCollider2D>();

        List<Collider2D> interactableObjects = new List<Collider2D>();
        Physics2D.OverlapCollider(capsuleTriggerZone, new ContactFilter2D(), interactableObjects);

        //On vérifie si une table se trouve dans notre champs d'action
        if (interactableObjects.Any(o => o.transform.CompareTag("Table")))
        {
            var tables = interactableObjects.Where(o => o.transform.CompareTag("Table"));
            foreach(var table in tables)
            {
                TableBehaviour tableBehaviour = table.GetComponent<TableBehaviour>();
                if (tableBehaviour.CanAcceptObject())
                {
                    tableBehaviour.SetObjectCarried(objectCarried);
                    objectCarried = null;
                    return;
                }
            }
        }
        else
        {
            objectCarried.GetComponent<Collider2D>().isTrigger = false;
            objectCarried = null;
        }
    }
}

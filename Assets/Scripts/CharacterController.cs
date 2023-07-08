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
    private Rigidbody2D rigidbodyCharacter;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Transform objectCarried;
    private Transform interactTriggerZone;

    private float currentSpeed;
    private float timeSinceLastDash;

    //Data from last frame for calculation and animation
    private float lastDirectionAngle;

    [SerializeField]
    private float objectCarriedDistanceFactor = 0.7f;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float dashSpeed;

    [SerializeField]
    private float dashTime;


    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Redomino.Action.performed += ctx => Interact();
        playerInput.Redomino.Back.performed += ctx => Dash();
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbodyCharacter = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        interactTriggerZone = transform.GetChild(0);

        currentSpeed = speed;
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
            if (movement.x > 0.5 || movement.x < -0.5)
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
            rigidbodyCharacter.MovePosition(rigidbodyCharacter.position + movement * currentSpeed * Time.deltaTime);

            //Gestion du dash
            //if(Time.)
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
                Vector3 direction = lastDirectionAngle == 0 ? Vector3.down : lastDirectionAngle == 180 ? Vector3.up : lastDirectionAngle == 90 ? Vector3.right : Vector3.left;
                objectCarried.position = transform.position + direction * objectCarriedDistanceFactor;
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

    void Dash()
    {
        
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
            var tables = interactableObjects.Where(o => o.transform.CompareTag("Table"));

            //Dans le cas ou il y a plusieurs tables on trie par leurs distance
            tables = tables.OrderBy(t => Vector2.Distance(t.transform.position, transform.position));

            foreach (var table in tables)
            {
                TableBehaviour tableBehaviour = table.GetComponent<TableBehaviour>();
                if (!tableBehaviour.CanAcceptObject())
                {
                    objectCarried = tableBehaviour.GetObjectCarried();
                    return;
                }
            }
        }

        //Dans le dernier cas on vérifie si on ne peux pas r�cup�rer un bloc proche autour du joueur
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
        objectCarried.GetComponent<SpriteRenderer>().sortingOrder = 5;

        CapsuleCollider2D c = new CapsuleCollider2D();

        var capsuleTriggerZone = interactTriggerZone.GetComponent<CapsuleCollider2D>();

        List<Collider2D> interactableObjects = new List<Collider2D>();
        Physics2D.OverlapCollider(capsuleTriggerZone, new ContactFilter2D(), interactableObjects);

        //On vérifie si une table se trouve dans notre champs d'action
        if (interactableObjects.Any(o => o.transform.CompareTag("Table")))
        {
            var tables = interactableObjects.Where(o => o.transform.CompareTag("Table"));
            
            //Dans le cas ou il y a plusieurs tables on trie par leurs distance
            tables = tables.OrderBy(t => Vector2.Distance(t.transform.position, transform.position));
    
            foreach (var table in tables)
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

        //On vérifie que l'objet ne tombe pas dans un mur
        var objectCarriedCollider = objectCarried.GetComponent<Collider2D>();
        List<Collider2D> collidersWhenDropObjectCarried = new List<Collider2D>();
        Physics2D.OverlapCollider(objectCarriedCollider, new ContactFilter2D(), collidersWhenDropObjectCarried);

        //Ici on compare à la fois les mur et les tables, car si on arrive à ce point dans la fonction c'est que toutes les tables sont prises
        if(collidersWhenDropObjectCarried.Any(c => c.transform.CompareTag("Walls") || c.transform.CompareTag("Table")))
        {
            objectCarried.position = transform.position;
        }

        objectCarriedCollider.isTrigger = false;
        objectCarried = null;
        
    }
}

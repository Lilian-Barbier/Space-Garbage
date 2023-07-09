using Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class CharacterController : MonoBehaviour
{

    private bool isStopped;
    private Rigidbody2D rigidbodyCharacter;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AssemblerManager assemblerManager;

    //A retirer
    private GameObject assembler;

    private Transform objectCarried;
    private Transform objectDetected;
    private Transform interactTriggerZone;

    //Data from last frame for calculation and animation
    private float lastDirectionAngle;
    private Vector2 lastDirection;

    [SerializeField] Animator redomino;
    [SerializeField] Animator bludomino;

    [SerializeField]
    private float objectCarriedDistanceFactor = 0.7f;
    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private float speed;

    [SerializeField] float dashSpeed = 0.5f;

    [SerializeField] float dashDuration = 0.5f;
    [SerializeField] float dashCooldown = 1f;

    private bool isDashing = false;
    private bool canDash = true;
    private float dashTimer;


    public int playerNumber;

    private bool IsInAssembler;

    private bool alreadyMoveInAssembler;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitObject();
    }

    void Start()
    {
        InitObject();
    }

    private void InitObject()
    {
        if (SceneManager.GetActiveScene().name.Contains("Level"))
        {
            rigidbodyCharacter = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            interactTriggerZone = transform.GetChild(0);
            assembler = GameObject.FindGameObjectWithTag("Assembler");
            assemblerManager = assembler.GetComponent<AssemblerManager>();

            if (playerNumber == 0)
            {
                transform.position = GameObject.FindGameObjectWithTag("SpawnP1").transform.position;
                animator.SetTrigger("isRed");
            }
            else
            {
                transform.position = GameObject.FindGameObjectWithTag("SpawnP2").transform.position;
                animator.SetTrigger("isBlue");
            }

        }
    }

    private Vector2 movement = Vector2.zero;
    public void Move(InputAction.CallbackContext ctx)
    {
        movement = ctx.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name.Contains("Level"))
        {
            if (IsInAssembler)
            {

            }
            else
            {
                #region In movement
                var isMovingSide = false;
                var isMovingUp = false;
                var isMovingDown = false;

                if (!isStopped)
                {
                    dashTimer += Time.deltaTime;

                    if(dashTimer > dashCooldown)
                    {
                        canDash = true;
                    }

                    if (isDashing)
                    {
                        rigidbodyCharacter.MovePosition(rigidbodyCharacter.position + lastDirection * dashSpeed * Time.deltaTime);
                        

                        if(dashTimer > dashDuration)
                        {
                            isDashing = false;
                        }
                    }
                    else
                    {
                        lastDirection = movement;
                        if (movement.x > 0.5 || movement.x < -0.5)
                        {
                            isMovingSide = true;
                            spriteRenderer.flipX = movement.x < 0;
                            lastDirectionAngle = movement.x > 0 ? 90 : -90;
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
                        rigidbodyCharacter.MovePosition(rigidbodyCharacter.position + movement * speed * Time.deltaTime);

                    }

                }

                animator.SetBool("isMovingSide", isMovingSide);
                animator.SetBool("isMovingUp", isMovingUp);
                animator.SetBool("isMovingDown", isMovingDown);

                if (objectCarried != null)
                {
                    if (movement != Vector2.zero)
                    {
                        objectCarried.position = transform.position + (Vector3)movement.normalized * objectCarriedDistanceFactor + offset;
                        objectCarried.GetComponent<SpriteRenderer>().sortingOrder = movement.y < 0 && movement.x < 0.4 && movement.x > -0.4 ? 15 : 5;
                    }
                    else
                    {
                        Vector3 direction = lastDirectionAngle == 0 ? Vector3.down : lastDirectionAngle == 180 ? Vector3.up : lastDirectionAngle == 90 ? Vector3.right : Vector3.left;
                        objectCarried.position = transform.position + direction * objectCarriedDistanceFactor + offset;
                        objectCarried.GetComponent<SpriteRenderer>().sortingOrder = lastDirectionAngle == 180 ? 5 : 15;
                    }

                }

                #endregion In movement
            
                UpdateOutlines();
            }
        }
    }

    private void UpdateOutlines() {
      CapsuleCollider2D c = new CapsuleCollider2D();

      var capsuleTriggerZone = interactTriggerZone.GetComponent<CapsuleCollider2D>();

      List<Collider2D> interactableObjects = new List<Collider2D>();

      ContactFilter2D contactFilter = new ContactFilter2D();
      contactFilter.useTriggers = true;

      Physics2D.OverlapCollider(capsuleTriggerZone, contactFilter, interactableObjects);

      //D'abord on vérifie si on peut prendre un bloc devant nous
      if (interactableObjects.Any(o => o.transform.CompareTag("Blocks")))
      {
          var collider = interactableObjects.First(o => o.transform.CompareTag("Blocks"));
          var objectDetected = collider.transform;

          if (objectDetected != this.objectDetected && this.objectDetected != null)
            this.objectDetected.GetComponent<SpriteRenderer>().material.SetInt("_IsDetected", 0);

          objectDetected.GetComponent<SpriteRenderer>().material.SetInt("_IsDetected", 1);
          this.objectDetected = objectDetected;

          return;
      }
      
      if(this.objectDetected != null) {
        this.objectDetected.GetComponent<SpriteRenderer>().material.SetInt("_IsDetected", 0);
        this.objectDetected = null;
      }
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (SceneManager.GetActiveScene().name.Contains("Level"))
        {
            if (ctx.performed)
            {
                if (IsInAssembler)
                {
                    var domino = objectCarried.GetComponent<DominoBehavior>().domino;
                    if (assemblerManager.CanAddDomino(domino))
                    {
                        assemblerManager.AddDomino(domino);
                        objectCarried.gameObject.SetActive(false);
                        objectCarried = null;
                        IsInAssembler = false;
                    }
                }
                else
                {
                    var circleTriggerZone = interactTriggerZone.GetComponent<CircleCollider2D>();

                    var interactableObjects = new List<Collider2D>();
                    var contactFilter = new ContactFilter2D();
                    contactFilter.useTriggers = true;
                    Physics2D.OverlapCollider(circleTriggerZone, contactFilter, interactableObjects);

                    if (objectCarried != null)
                    {
                        if (interactableObjects.Any(o => o.transform.CompareTag("AssemblerButton")))
                        {
                            var domino = objectCarried.GetComponent<DominoBehavior>();
                            assemblerManager.CreateSpriteForAddedDomino(domino.domino);

                            objectCarried.position = new Vector3(100, 100);
                            IsInAssembler = true;
                        }
                        else
                        {
                            DropObject();
                        }
                    }
                    else
                    {
                        if (interactableObjects.Any(o => o.transform.CompareTag("Assembler")) && !assemblerManager.isEmpty())
                        {
                            objectCarried = assemblerManager.GetDomino().transform;
                            objectCarried.GetComponent<Collider2D>().isTrigger = true;
                        }
                        else
                        {
                            GetObjectNear();
                        }
                    }

                }
            }

        }
    }

    public void Dash()
    {
        if (canDash)
        {
            canDash = false;
            isDashing = true;
            dashTimer = 0f;
        }
    }

    public void MoveDominosInAssembler(InputAction.CallbackContext ctx)
    {
        if (SceneManager.GetActiveScene().name.Contains("Level"))
        {
            if (ctx.performed)
            {

                if (IsInAssembler && !alreadyMoveInAssembler)
                {
                    alreadyMoveInAssembler = true;

                    var domino = objectCarried.GetComponent<DominoBehavior>();

                    Vector2 movement = ctx.ReadValue<Vector2>();

                    if (movement.x > 0.5)
                    {
                        domino.MoveDominoRight();
                    }
                    else if (movement.x < -0.5)
                    {
                        domino.MoveDominoLeft();
                    }
                    else if (movement.y < 0)
                    {
                        domino.MoveDominoDown();
                    }
                    else if (movement.y > 0)
                    {
                        domino.MoveDominoUp();
                    }

                    assemblerManager.CreateSpriteForAddedDomino(domino.domino);
                }

            }
            else if (ctx.canceled)
            {
                alreadyMoveInAssembler = false;
            }

        }
    }

    public void RotateClockwise(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (objectCarried != null)
            {
                var domino = objectCarried.GetComponent<DominoBehavior>();
                domino.RotateDominoClockwise();
                if (IsInAssembler)
                {
                    assemblerManager.CreateSpriteForAddedDomino(domino.domino);
                }
            }
        }
    }

    public void RotateCounterClockwise(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (objectCarried != null)
            {
                var domino = objectCarried.GetComponent<DominoBehavior>();
                domino.RotateDominoCounterClockwise();
                if (IsInAssembler)
                {
                    assemblerManager.CreateSpriteForAddedDomino(domino.domino);
                }
            }
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
        if (interactableObjects.Any(o => o.transform.CompareTag("Table")))
        {
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

        //Sinon on regarde si on peut prendre un domino d'une caisse
        if (interactableObjects.Any(o => o.transform.CompareTag("Box")))
        {
            var boxes = interactableObjects.Where(o => o.transform.CompareTag("Box"));

            //Dans le cas ou il y a plusieurs tables on trie par leurs distance
            boxes = boxes.OrderBy(t => Vector2.Distance(t.transform.position, transform.position));

            foreach (var box in boxes)
            {
                BoxBehaviour boxBehaviour = box.GetComponent<BoxBehaviour>();
                objectCarried = boxBehaviour.GetObject();
                return;
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

        //On vérifie si le point de livraison se trouve dans dans notre champs d'action 
        if (interactableObjects.Any(o => o.transform.CompareTag("DeliveryPoint")))
        {
            var deliveryPoint = interactableObjects.First(o => o.transform.CompareTag("DeliveryPoint")).GetComponent<DeliveryPointBehaviour>();
            var domino = objectCarried.GetComponent<DominoBehavior>().domino;

            deliveryPoint.DeliveryDomino(domino);
            objectCarried.gameObject.SetActive(false);
            objectCarried = null;
            return;
        }

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
        if (collidersWhenDropObjectCarried.Any(c => c.transform.CompareTag("Walls") || c.transform.CompareTag("Table")))
        {
            objectCarried.position = transform.position;
        }

        objectCarriedCollider.isTrigger = false;
        objectCarried = null;

    }
}

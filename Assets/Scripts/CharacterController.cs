using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterController : MonoBehaviour
{

    bool isStopped;
    Rigidbody2D rigidbodyCharacter;
    Animator animator;
    SpriteRenderer spriteRenderer;
    AssemblerManager assemblerManager;

    Transform objectCarried;
    Transform objectDetected;

    bool isDashing = false;
    bool canDash = true;
    float dashTimer;

    bool IsInAssembler;
    bool alreadyMoveInAssembler;
    
    Transform interactTriggerZone;
    Vector2 movement = Vector2.zero;

    //Data from last frame for calculation and animation
    Vector2 lastDirection;

    [SerializeField] float objectCarriedDistanceFactor = 0.7f;
    [SerializeField] Vector3 offsetObjectCarriedDistance;

    [SerializeField] float speed;
    [SerializeField] float dashSpeed = 0.5f;

    [SerializeField] float dashDuration = 0.5f;
    [SerializeField] float dashCooldown = 1f;

    public PlayerInput playerInput { get; set; }

    void Start()
    {
        rigidbodyCharacter = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        var assembler = GameObject.FindGameObjectWithTag("Assembler");
        assemblerManager = assembler.GetComponent<AssemblerManager>();

        interactTriggerZone = transform.GetChild(0);
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        movement = ctx.ReadValue<Vector2>();
    }

    void FixedUpdate()
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

                if (dashTimer > dashCooldown)
                {
                    canDash = true;
                }

                if (isDashing)
                {
                    rigidbodyCharacter.MovePosition(rigidbodyCharacter.position + dashSpeed * Time.deltaTime * lastDirection);


                    if (dashTimer > dashDuration)
                    {
                        isDashing = false;
                    }
                }
                else
                {
                    if(movement != Vector2.zero && movement.magnitude > 0.8)
                    {
                        lastDirection = movement;
                    }

                    if (movement.x > 0.5 || movement.x < -0.5)
                    {
                        isMovingSide = true;
                        spriteRenderer.flipX = movement.x < 0;
                    }
                    else if (movement.y < 0)
                    {
                        isMovingDown = true;
                    }
                    else if (movement.y > 0)
                    {
                        isMovingUp = true;
                    }

                    interactTriggerZone.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.down, lastDirection));
                    rigidbodyCharacter.MovePosition(rigidbodyCharacter.position + speed * Time.deltaTime * movement);

                }

            }

            animator.SetBool("isMovingSide", isMovingSide);
            animator.SetBool("isMovingUp", isMovingUp);
            animator.SetBool("isMovingDown", isMovingDown);

            if (objectCarried != null)
            {
                if (movement != Vector2.zero)
                {
                    objectCarried.position = transform.position + (Vector3)movement.normalized * objectCarriedDistanceFactor + offsetObjectCarriedDistance;
                    objectCarried.GetComponent<SpriteRenderer>().sortingOrder = movement.y < 0 && movement.x < 0.4 && movement.x > -0.4 ? 15 : 5;
                }
                else
                {
                    objectCarried.position = transform.position + (Vector3)(lastDirection * objectCarriedDistanceFactor) + offsetObjectCarriedDistance;
                    objectCarried.GetComponent<SpriteRenderer>().sortingOrder = lastDirection.y > 0.5 ? 5 : 15;
                }

            }

            #endregion In movement

            UpdateOutlines();
        }
    }

    void UpdateOutlines()
    {
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

        if (this.objectDetected != null)
        {
            this.objectDetected.GetComponent<SpriteRenderer>().material.SetInt("_IsDetected", 0);
            this.objectDetected = null;
        }
    }

    public void Interact(InputAction.CallbackContext ctx)
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

    void GetObjectNear()
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

    void DropObject()
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


    public void LoadChooseLevel(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            SceneManager.LoadScene("ChooseLvl");
        }
    }
}

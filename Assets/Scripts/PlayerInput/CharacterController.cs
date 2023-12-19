using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Machines;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class CharacterController : MonoBehaviour
{
    Rigidbody2D rigidbodyCharacter;
    Animator animator;
    SpriteRenderer spriteRenderer;

    AssemblerBehavior assembler;

    Transform objectCarried;
    Transform objectDetected;

    bool isStopped;
    bool isDashing = false;
    bool canDash = true;
    float dashTimer;

    HologramBehavior assemblerHologram;

    Transform interactTriggerZone;
    Vector2 movement = Vector2.zero;

    //Data from last frame for calculation and animation
    Vector2 lastDirection;

    [SerializeField] float objectCarriedDistanceFactor;
    [SerializeField] Vector3 offsetObjectCarriedDistance;

    [SerializeField] float speed;
    [SerializeField] float dashSpeed;

    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;

    [SerializeField] TileBase tileConveyorRight;
    [SerializeField] TileBase tileConveyorLeft;
    [SerializeField] TileBase tileConveyorUp;
    [SerializeField] TileBase tileConveyorDown;

    [SerializeField] TileBase eraseTile;

    //Engines Tiles for hologram
    [SerializeField] TileBase filterTile;
    [SerializeField] TileBase fuelTile;
    [SerializeField] TileBase separatorTile;
    [SerializeField] TileBase fusionTile;


    //Engines prefab 
    [SerializeField] GameObject fusionMachinePrefab;
    [SerializeField] GameObject separatorPrefab;
    [SerializeField] GameObject fuelMachinePrefab;
    [SerializeField] GameObject filterPrefab;

    private Tilemap conveyorBeltRight;
    private Tilemap conveyorBeltLeft;
    private Tilemap conveyorBeltDown;
    private Tilemap conveyorBeltUp;
    private Tilemap conveyorBeltHologram;

    private Direction conveyorDirection = Direction.Right;

    public PlayerAction selectedAction;

    [SerializeField] float constructionDistanceRatio = 0.6f;

    bool isInComputer = false;
    bool isInHookMode = false;
    HookButtonController hookController;

    public int nbOfConveyorBelt = 4;
    public int nbOfFusionMachine = 0;
    public int nbOfSeparator = 0;
    public int nbOfFuelMachine = 0;
    public int nbOfFilter = 1;

    public bool userUseKeyboard = true;

    SpaceshipManager spaceshipManager;

    public enum PlayerAction
    {
        TakeObject,
        RemoveConstruction,
        ConstructConveyor,
        ConstructFusionMachine,
        ConstructSeparator,
        ConstructFuelMachine,
        ConstructFilter,
    }

    enum Direction
    {
        Right,
        Left,
        Up,
        Down
    }

    void Start()
    {
        rigidbodyCharacter = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // var assemblerGameObject = GameObject.FindGameObjectWithTag("Assembler");
        // assembler = assemblerGameObject.GetComponent<AssemblerBehavior>();

        interactTriggerZone = transform.GetChild(0);

        conveyorBeltRight = GameObject.FindGameObjectWithTag("ConveyorBeltRight").GetComponent<Tilemap>();
        conveyorBeltLeft = GameObject.FindGameObjectWithTag("ConveyorBeltLeft").GetComponent<Tilemap>();
        conveyorBeltUp = GameObject.FindGameObjectWithTag("ConveyorBeltUp").GetComponent<Tilemap>();
        conveyorBeltDown = GameObject.FindGameObjectWithTag("ConveyorBeltDown").GetComponent<Tilemap>();
        conveyorBeltHologram = GameObject.FindGameObjectWithTag("ConveyorBeltHologram").GetComponent<Tilemap>();

        selectedAction = PlayerAction.TakeObject;

        spaceshipManager = FindObjectOfType<SpaceshipManager>().GetComponent<SpaceshipManager>();
    }

    void FixedUpdate()
    {
        if (assemblerHologram != null) return;

        UpdateMovements();

        if (selectedAction == PlayerAction.TakeObject)
            UpdateOutlines();

        else if (selectedAction == PlayerAction.ConstructConveyor)
            UpdateHologramConstruction();

        else if (selectedAction == PlayerAction.RemoveConstruction)
            UpdateHologram(eraseTile);

        else if (selectedAction == PlayerAction.ConstructFusionMachine)
            UpdateHologram(fusionTile);

        else if (selectedAction == PlayerAction.ConstructSeparator)
            UpdateHologram(separatorTile);

        else if (selectedAction == PlayerAction.ConstructFuelMachine)
            UpdateHologram(fuelTile);

        else if (selectedAction == PlayerAction.ConstructFilter)
            UpdateHologram(filterTile);
    }

    void UpdateHologramConstruction()
    {
        TileBase selectedConveyorTile = tileConveyorRight;
        switch (conveyorDirection)
        {
            case Direction.Right:
                selectedConveyorTile = tileConveyorRight;
                break;
            case Direction.Left:
                selectedConveyorTile = tileConveyorLeft;
                break;
            case Direction.Up:
                selectedConveyorTile = tileConveyorUp;
                break;
            case Direction.Down:
                selectedConveyorTile = tileConveyorDown;
                break;
        }

        conveyorBeltHologram.ClearAllTiles();

        var tilePos = transform.position + (Vector3)lastDirection.normalized * constructionDistanceRatio;
        conveyorBeltHologram.SetTile(conveyorBeltHologram.WorldToCell(tilePos), selectedConveyorTile);
    }

    void UpdateHologram(TileBase tile)
    {
        conveyorBeltHologram.ClearAllTiles();
        var tilePos = transform.position + (Vector3)lastDirection.normalized * constructionDistanceRatio;
        conveyorBeltHologram.SetTile(conveyorBeltHologram.WorldToCell(tilePos), tile);
    }

    void ClearHologramConstruction()
    {
        conveyorBeltHologram.ClearAllTiles();
    }

    void UpdateMovements()
    {
        var isMovingSide = false;
        var isMovingUp = false;
        var isMovingDown = false;

        if (!isStopped)
        {
            dashTimer += Time.fixedDeltaTime;

            if (dashTimer > dashCooldown)
            {
                canDash = true;
            }

            if (isDashing)
            {
                rigidbodyCharacter.MovePosition(rigidbodyCharacter.position + dashSpeed * Time.fixedDeltaTime * lastDirection);


                if (dashTimer > dashDuration)
                {
                    isDashing = false;
                }
            }
            else
            {
                if (movement != Vector2.zero && movement.magnitude > 0.8)
                {
                    lastDirection = movement;
                }

                if (movement.y < -0.5)
                {
                    isMovingDown = true;
                }
                else if (movement.y > 0.5)
                {
                    isMovingUp = true;
                }
                else if (movement.x != 0)
                {
                    isMovingSide = true;
                    spriteRenderer.flipX = movement.x < 0;
                }

                interactTriggerZone.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.down, lastDirection));
                rigidbodyCharacter.MovePosition(rigidbodyCharacter.position + speed * Time.fixedDeltaTime * movement);

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
    }

    #region InputAction CallBack
    public void Move(InputAction.CallbackContext ctx)
    {
        OnAction(ctx);

        movement = ctx.ReadValue<Vector2>();
    }
    public void Dash(InputAction.CallbackContext ctx)
    {
        OnAction(ctx);

        if (!ctx.performed) return;

        if (isInHookMode)
        {
            isInHookMode = false;
            isStopped = false;
        }
        else if (isInComputer)
        {
            isInComputer = false;
            isStopped = false;
            spaceshipManager.HideComputer();
        }
        else if (canDash)
        {
            CameraShake.Instance.LittleShake();
            canDash = false;
            isDashing = true;
            dashTimer = 0f;
        }
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        OnAction(ctx);

        if (!ctx.performed) return;

        if (isInHookMode)
        {
            hookController.LaunchHook();
        }
        else if (isInComputer)
        {
            spaceshipManager.BuyUpgrade();
        }
        else if (assemblerHologram != null)
        {
            QuitAssemblerAndInsertDomino();
        }
        else
        {
            //In construction mode : 
            if (selectedAction == PlayerAction.ConstructConveyor && nbOfConveyorBelt > 0 && !AlreadyConstructFront())
            {
                TileBase selectedConveyorTile = tileConveyorRight;
                Tilemap selectedTilemap = conveyorBeltRight;

                switch (conveyorDirection)
                {
                    case Direction.Right:
                        selectedConveyorTile = tileConveyorRight;
                        selectedTilemap = conveyorBeltRight;
                        break;
                    case Direction.Left:
                        selectedConveyorTile = tileConveyorLeft;
                        selectedTilemap = conveyorBeltLeft;
                        break;
                    case Direction.Up:
                        selectedConveyorTile = tileConveyorUp;
                        selectedTilemap = conveyorBeltUp;
                        break;
                    case Direction.Down:
                        selectedConveyorTile = tileConveyorDown;
                        selectedTilemap = conveyorBeltDown;
                        break;
                }

                var tilePos = transform.position + (Vector3)lastDirection.normalized * constructionDistanceRatio;
                selectedTilemap.SetTile(selectedTilemap.WorldToCell(tilePos), selectedConveyorTile);
                nbOfConveyorBelt--;
            }

            if (selectedAction == PlayerAction.ConstructFusionMachine && nbOfFusionMachine > 0 && !AlreadyConstructFront())
            {
                Instantiate(fusionMachinePrefab, GetPositionOnTilemap(), Quaternion.identity);
                nbOfFusionMachine--;
            }

            if (selectedAction == PlayerAction.ConstructSeparator && nbOfSeparator > 0 && !AlreadyConstructFront())
            {
                Instantiate(separatorPrefab, GetPositionOnTilemap(), Quaternion.identity);
                nbOfSeparator--;
            }

            if (selectedAction == PlayerAction.ConstructFuelMachine && nbOfFuelMachine > 0 && !AlreadyConstructFront())
            {
                Instantiate(fuelMachinePrefab, GetPositionOnTilemap(), Quaternion.identity);
                nbOfFuelMachine--;
            }

            if (selectedAction == PlayerAction.ConstructFilter && nbOfFilter > 0 && !AlreadyConstructFront())
            {
                Instantiate(filterPrefab, GetPositionOnTilemap(), Quaternion.identity);
                nbOfFilter--;
            }

            else if (selectedAction == PlayerAction.RemoveConstruction)
            {
                var tileRemove = conveyorBeltRight.WorldToCell(transform.position + (Vector3)lastDirection.normalized * constructionDistanceRatio);

                if (conveyorBeltRight.GetTile(tileRemove) != null)
                    nbOfConveyorBelt++;
                else if (conveyorBeltLeft.GetTile(tileRemove) != null)
                    nbOfConveyorBelt++;
                else if (conveyorBeltUp.GetTile(tileRemove) != null)
                    nbOfConveyorBelt++;
                else if (conveyorBeltDown.GetTile(tileRemove) != null)
                    nbOfConveyorBelt++;

                conveyorBeltRight.SetTile(tileRemove, null);
                conveyorBeltLeft.SetTile(tileRemove, null);
                conveyorBeltUp.SetTile(tileRemove, null);
                conveyorBeltDown.SetTile(tileRemove, null);

                Vector2 positionToCheck = new Vector2(tileRemove.x + 0.5f, tileRemove.y + 0.5f);
                Collider2D hitCollider = Physics2D.OverlapPoint(positionToCheck);

                if (hitCollider != null &&
                    hitCollider.transform.CompareTag("Table") &&
                    hitCollider.name != "DeliveryPointFuel" &&
                    hitCollider.name != "DeliveryPoint")
                {
                    if (hitCollider.transform.GetComponent<FusionBehaviour>() != null)
                    {
                        nbOfFusionMachine++;
                    }
                    else if (hitCollider.transform.GetComponent<SeparatorBehaviour>() != null)
                    {
                        nbOfSeparator++;
                    }
                    else if (hitCollider.transform.GetComponent<FuelCreatorBehaviour>() != null)
                    {
                        nbOfFuelMachine++;
                    }
                    else if (hitCollider.transform.GetComponent<FilterBehaviour>() != null)
                    {
                        nbOfFilter++;
                    }

                    Destroy(hitCollider.gameObject);
                }
            }

            else if (selectedAction == PlayerAction.TakeObject)
            {
                var interactibleObjectsNear = GetColliderAroundPlayerOrderByDistance(getTriggerCollider: true);
                var interactibleObjectsInFront = GetColliderInFrontPlayerOrderByDistance();

                //Si le joueur à un objet il va le déposer
                if (objectCarried != null)
                {
                    if (interactibleObjectsNear.Any(o => o.transform.CompareTag("AssemblerButton")))
                        AddHologramToAssembler();
                    else
                        DropObject();
                }
                //Sinon il va tenter d'en attraper un
                else
                {
                    if (interactibleObjectsNear.Any(o => o.transform.CompareTag("GrapplingHookButton")))
                    {
                        isInHookMode = true;
                        isStopped = true;
                        hookController = interactibleObjectsNear.First(o => o.transform.CompareTag("GrapplingHookButton")).GetComponent<HookButtonController>();
                    }
                    if (interactibleObjectsNear.Any(o => o.transform.CompareTag("ComputerButton")))
                    {
                        isInComputer = true;
                        isStopped = true;
                        spaceshipManager.ShowComputer();
                    }
                    else if (interactibleObjectsInFront.Any(o => o.transform.CompareTag("Table")) && interactibleObjectsInFront.First(o => o.transform.CompareTag("Table")).GetComponent<FilterBehaviour>() != null)
                    {
                        var filter = interactibleObjectsInFront.First(o => o.transform.CompareTag("Table")).GetComponent<FilterBehaviour>();
                        filter.ChangeFilterSize();
                    }

                    else
                    {
                        GetObjectNear();
                    }
                }
            }
        }
    }

    private Vector3 GetPositionOnTilemap()
    {
        var tilePos = transform.position + (Vector3)lastDirection.normalized * constructionDistanceRatio;
        var cellPos = conveyorBeltHologram.WorldToCell(tilePos);
        tilePos = conveyorBeltHologram.CellToWorld(cellPos);
        tilePos += new Vector3(0.5f, 0.5f, 0);
        return tilePos;
    }

    private bool AlreadyConstructFront()
    {
        var cellPos = conveyorBeltDown.WorldToCell(transform.position + (Vector3)lastDirection.normalized * constructionDistanceRatio);

        if (conveyorBeltDown.GetTile(cellPos) != null || conveyorBeltUp.GetTile(cellPos) != null || conveyorBeltRight.GetTile(cellPos) != null || conveyorBeltLeft.GetTile(cellPos) != null)
        {
            return true;
        }

        Vector2 positionToCheck = new Vector2(cellPos.x + 0.5f, cellPos.y + 0.5f);
        Collider2D hitCollider = Physics2D.OverlapPoint(positionToCheck);

        if (hitCollider != null && !hitCollider.transform.CompareTag("Player") && !hitCollider.transform.CompareTag("Blocks"))
        {
            return true;
        }

        return false;
    }

    public void MoveDominosInAssembler(InputAction.CallbackContext ctx)
    {
        OnAction(ctx);

        if (!ctx.started) return;

        if (assemblerHologram != null)
        {
            var domino = objectCarried.GetComponent<TrashBehaviour>();

            Vector2 movement = ctx.ReadValue<Vector2>();

            if (movement.x > 0.5)
                domino.MoveDominoRight();
            else if (movement.x < -0.5)
                domino.MoveDominoLeft();
            else if (movement.y < 0)
                domino.MoveDominoDown();
            else if (movement.y > 0)
                domino.MoveDominoUp();

            assemblerHologram.SetDomino(domino.trash);
        }
        else if (isInHookMode)
        {
            Vector2 movement = ctx.ReadValue<Vector2>();

            if (movement.y < -0.2)
                hookController.UpHook();
            else if (movement.y > 0.2)
                hookController.DownHook();
        }
        else if (isInComputer)
        {
            Vector2 movement = ctx.ReadValue<Vector2>();

            if (movement.y < -0.2)
                spaceshipManager.SelectNextComputer();
            else if (movement.y > 0.2)
                spaceshipManager.SelectPreviousComputer();
        }
    }

    public void RotateClockwise(InputAction.CallbackContext ctx)
    {
        OnAction(ctx);

        if (!ctx.performed) return;

        if (selectedAction == PlayerAction.ConstructConveyor)
            RotateConveyor(true);

        if (selectedAction == PlayerAction.TakeObject)
            RotateDomino(true);
    }

    public void RotateCounterClockwise(InputAction.CallbackContext ctx)
    {
        OnAction(ctx);

        if (!ctx.performed) return;

        if (selectedAction == PlayerAction.ConstructConveyor)
            RotateConveyor(false);

        if (selectedAction == PlayerAction.TakeObject)
            RotateDomino(false);
    }

    public void ChangeAction(InputAction.CallbackContext ctx)
    {
        OnAction(ctx);

        if (!ctx.performed) return;

        switch (selectedAction)
        {
            case PlayerAction.ConstructFuelMachine:
                ClearHologramConstruction();
                selectedAction = PlayerAction.TakeObject;
                break;

            case PlayerAction.ConstructFusionMachine:
                ClearHologramConstruction();
                selectedAction = PlayerAction.ConstructFuelMachine;
                break;

            case PlayerAction.ConstructSeparator:
                ClearHologramConstruction();
                selectedAction = PlayerAction.ConstructFusionMachine;
                break;

            case PlayerAction.ConstructFilter:
                ClearHologramConstruction();
                selectedAction = PlayerAction.ConstructSeparator;
                break;

            case PlayerAction.ConstructConveyor:
                ClearHologramConstruction();
                selectedAction = PlayerAction.ConstructFilter;
                break;

            case PlayerAction.RemoveConstruction:
                ClearHologramConstruction();
                selectedAction = PlayerAction.ConstructConveyor;
                break;

            case PlayerAction.TakeObject:
                if (objectCarried != null)
                    DropObject();

                selectedAction = PlayerAction.RemoveConstruction;
                break;
        };

    }

    public void ChangeActionPrevious(InputAction.CallbackContext ctx)
    {
        OnAction(ctx);

        if (!ctx.performed) return;

        switch (selectedAction)
        {
            case PlayerAction.ConstructFuelMachine:
                ClearHologramConstruction();
                selectedAction = PlayerAction.ConstructFusionMachine;
                break;

            case PlayerAction.ConstructFusionMachine:
                ClearHologramConstruction();
                selectedAction = PlayerAction.ConstructSeparator;
                break;

            case PlayerAction.ConstructSeparator:
                ClearHologramConstruction();
                selectedAction = PlayerAction.ConstructFilter;
                break;

            case PlayerAction.ConstructFilter:
                ClearHologramConstruction();
                selectedAction = PlayerAction.ConstructConveyor;
                break;

            case PlayerAction.ConstructConveyor:
                ClearHologramConstruction();
                selectedAction = PlayerAction.RemoveConstruction;
                break;

            case PlayerAction.RemoveConstruction:
                ClearHologramConstruction();
                selectedAction = PlayerAction.TakeObject;
                break;

            case PlayerAction.TakeObject:
                if (objectCarried != null)
                    DropObject();
                ClearHologramConstruction();


                selectedAction = PlayerAction.ConstructFuelMachine;
                break;
        };

    }

    private void RotateConveyor(bool clockwise)
    {
        switch (conveyorDirection)
        {
            case Direction.Right:
                conveyorDirection = clockwise ? Direction.Down : Direction.Up;
                break;
            case Direction.Left:
                conveyorDirection = clockwise ? Direction.Up : Direction.Down;
                break;
            case Direction.Up:
                conveyorDirection = clockwise ? Direction.Right : Direction.Left;
                break;
            case Direction.Down:
                conveyorDirection = clockwise ? Direction.Left : Direction.Right;
                break;
        }
    }

    private void RotateDomino(bool clockwise)
    {
        if (objectCarried == null) return;

        var domino = objectCarried.GetComponent<TrashBehaviour>();

        if (clockwise) domino.RotateDominoClockwise();
        else domino.RotateDominoCounterClockwise();

        if (assemblerHologram != null) assemblerHologram.SetDomino(domino.trash);
    }

    #endregion

    #region Interact with object utils functions
    void GetObjectNear()
    {
        IEnumerable<Collider2D> interactableObjects = GetColliderInFrontPlayerOrderByDistance();

        Collider2D block = GetNearestObjectInCollidersByTag(interactableObjects, "Blocks");

        //D'abord on vérifie si on peut prendre un bloc devant nous
        if (block != null)
        {
            block.isTrigger = true;
            objectCarried = block.transform;
            return;
        }

        //Sinon on essaie de prendre un bloc d'une table
        TableBehaviour tableBehaviour = GetFirstTableWithObject(interactableObjects);
        if (tableBehaviour != null)
        {
            objectCarried = tableBehaviour.GetObjectCarried();
            return;
        }

        Collider2D box = GetNearestObjectInCollidersByTag(interactableObjects, "Crate");
        if (box != null)
        {
            CrateBehaviour boxBehaviour = box.GetComponent<CrateBehaviour>();
            objectCarried = boxBehaviour.GetObject();
            return;
        }


        //Dans le dernier cas on vérifie si on ne peux pas récupérer un bloc proche autour du joueur
        Collider2D blockAroundPlayer = GetNearestObjectInCollidersByTag(interactableObjects, "Blocks");
        if (blockAroundPlayer != null)
        {
            blockAroundPlayer.isTrigger = true;
            objectCarried = blockAroundPlayer.transform;
        }

    }

    private void AddHologramToAssembler()
    {
        var domino = objectCarried.GetComponent<TrashBehaviour>();
        assemblerHologram = assembler.AddHologram(domino.trash);
    }

    private void QuitAssemblerAndInsertDomino()
    {
        if (assembler.TryInsertHologram(assemblerHologram))
        {

            Destroy(objectCarried.gameObject);
            objectCarried = null;

            Destroy(assemblerHologram.gameObject);
            assemblerHologram = null;
        }
    }

    void DropObject()
    {
        objectCarried.GetComponent<SpriteRenderer>().sortingOrder = 5;

        IEnumerable<Collider2D> interactableObjects = GetColliderInFrontPlayerOrderByDistance();

        Collider2D deliveryPointTransform = GetNearestObjectInCollidersByTag(interactableObjects, "DeliveryPoint");

        //On vérifie si une table se trouve dans notre champs d'action
        if (interactableObjects.Any(o => o.transform.CompareTag("Table")))
        {
            var tables = interactableObjects.Where(o => o.transform.CompareTag("Table"));

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

        //On vérifie que l'objet ne tombe pas dans un mur, si c'est le cas on set la position à la position du joueur
        var objectCarriedCollider = objectCarried.GetComponent<Collider2D>();
        List<Collider2D> collidersWhenDropObjectCarried = new();
        Physics2D.OverlapCollider(objectCarriedCollider, new ContactFilter2D(), collidersWhenDropObjectCarried);

        //Ici on compare à la fois les mur et les tables, car si on arrive à ce point dans la fonction c'est que toutes les tables sont prises
        if (collidersWhenDropObjectCarried.Any(c => c.transform.CompareTag("Walls") || c.transform.CompareTag("Table")))
        {
            objectCarried.position = transform.position;
        }

        objectCarriedCollider.isTrigger = false;
        objectCarried = null;

    }

    //Récupére les Colliders que le joueur colle, dans sa zone Circle
    IEnumerable<Collider2D> GetColliderAroundPlayerOrderByDistance(bool getTriggerCollider = false)
    {
        var circleTriggerZone = interactTriggerZone.GetComponent<CircleCollider2D>();

        var colliders = new List<Collider2D>();
        var contactFilter = new ContactFilter2D
        {
            useTriggers = getTriggerCollider
        };
        Physics2D.OverlapCollider(circleTriggerZone, contactFilter, colliders);

        return colliders.OrderBy(c => Vector2.Distance(c.transform.position, transform.position)).ToList();
    }

    //Récupére les Colliders qui sont devant le joueur, dans sa zone Capsule
    IEnumerable<Collider2D> GetColliderInFrontPlayerOrderByDistance(bool getTriggerCollider = false)
    {
        var capsuleTriggerZone = interactTriggerZone.GetComponent<CapsuleCollider2D>();

        var colliders = new List<Collider2D>();
        var contactFilter = new ContactFilter2D
        {
            useTriggers = getTriggerCollider
        };
        Physics2D.OverlapCollider(capsuleTriggerZone, contactFilter, colliders);

        return colliders.OrderBy(c => Vector2.Distance(c.transform.position, transform.position)).ToList();
    }

    Collider2D GetNearestObjectInCollidersByTag(IEnumerable<Collider2D> colliders, string tagName)
    {
        //D'abord on vérifie si on peut prendre un bloc devant nous
        if (colliders.Any(o => o.transform.CompareTag(tagName)))
        {
            return colliders.First(o => o.transform.CompareTag(tagName));
        }

        return null;
    }

    TableBehaviour GetFirstTableWithObject(IEnumerable<Collider2D> colliders)
    {
        if (colliders.Any(o => o.transform.CompareTag("Table")))
        {
            var tables = colliders.Where(o => o.transform.CompareTag("Table"));

            foreach (var table in tables)
            {
                TableBehaviour tableBehaviour = table.GetComponent<TableBehaviour>();
                if (!tableBehaviour.CanAcceptObject())
                {
                    return tableBehaviour;
                }
            }
        }

        return null;
    }

    #endregion

    void UpdateOutlines()
    {
        IEnumerable<Collider2D> interactableObjects = GetColliderInFrontPlayerOrderByDistance();
        Collider2D block = GetNearestObjectInCollidersByTag(interactableObjects, "Blocks");
        TableBehaviour tableBehaviour = GetFirstTableWithObject(interactableObjects);

        if (block != null)
        {
            var newObjectOutlined = block.transform;

            if (newObjectOutlined != objectDetected && objectDetected != null)
                objectDetected.GetComponent<SpriteRenderer>().material.SetInt("_IsDetected", 0);

            newObjectOutlined.GetComponent<SpriteRenderer>().material.SetInt("_IsDetected", 1);
            objectDetected = newObjectOutlined;

        }

        //Check si une table se trouve devant nous, dans ce cas on on ajoute une outline sur l'objet dessus
        else if (tableBehaviour != null)
        {
            var newObjectOutlined = tableBehaviour.GetReferenceObjectCarried().transform;

            if (newObjectOutlined != objectDetected && objectDetected != null)
                objectDetected.GetComponent<SpriteRenderer>().material.SetInt("_IsDetected", 0);

            newObjectOutlined.GetComponent<SpriteRenderer>().material.SetInt("_IsDetected", 1);
            objectDetected = newObjectOutlined;
        }

        else if (objectDetected != null)
        {
            objectDetected.GetComponent<SpriteRenderer>().material.SetInt("_IsDetected", 0);
            objectDetected = null;
        }
    }

    //Todo : a passer dans une autre classe ? Oui 👍
    public void LoadChooseLevel(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            SceneManager.LoadScene("ChooseLvl");
        }
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        var device = context.control.device;

        if (device is Keyboard)
        {
            userUseKeyboard = true;
        }
        else if (device is Gamepad)
        {
            userUseKeyboard = false;
        }
    }

}

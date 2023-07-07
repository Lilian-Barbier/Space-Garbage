using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class CharacterController : MonoBehaviour
{
    private PlayerInput playerInput;
    private bool isStopped;
    private Rigidbody2D rigidbody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private float speed;

    private void Awake()
    {
        playerInput = new PlayerInput();
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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
            }
            else if (movement.y < 0)
            {
                isMovingDown = true;
            }
            else if (movement.y > 0)
            {
                isMovingUp = true;
            }

            rigidbody.MovePosition(rigidbody.position + movement * speed * Time.deltaTime);
        }

        animator.SetBool("isMovingSide", isMovingSide);
        animator.SetBool("isMovingUp", isMovingUp);
        animator.SetBool("isMovingDown", isMovingDown);
    }
}

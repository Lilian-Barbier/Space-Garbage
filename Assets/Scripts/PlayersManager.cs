
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayersManager : MonoBehaviour
{
    public List<UnityEngine.InputSystem.PlayerInput> playersInput = new();

    private PlayerInputManager inputManager;
    private int playersConnected = 0;

    public bool player1ControllerOk;
    public bool player2ControllerOk;

    public bool player1Ready;
    public bool player2Ready;

    private SpriteRenderer[] spriteRenderers;
    private Animator[] animators;

    private void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        spriteRenderers[0].color = new Color(0, 0, 0, 0);
        spriteRenderers[1].color = new Color(0, 0, 0, 0);

        animators = GetComponentsInChildren<Animator>();
        animators[0].SetTrigger("isRed");
        animators[1].SetTrigger("isBlue");
    }

    private void Awake()
    {
        inputManager = GetComponent<PlayerInputManager>();
    }

    private void OnEnable()
    {
        inputManager.onPlayerJoined += ctx => OnPlayerJoined(ctx);
    }


    private void OnDisable()
    {
        inputManager.onPlayerJoined -= OnPlayerJoined;
    }

    private void OnPlayerJoined(UnityEngine.InputSystem.PlayerInput playerInput)
    {
        playerInput.transform.position = new Vector3(0, -7);
        if (playersConnected == 0)
        {
            DontDestroyOnLoad(playerInput.gameObject);
            playerInput.GetComponent<CharacterController>().playerNumber = 0;
            playersInput.Add(playerInput);

            spriteRenderers[0].color = Color.white;
            animators[0].SetBool("isMovingDown", true);
        }
        else
        {
            DontDestroyOnLoad(playerInput.gameObject);
            playerInput.GetComponent<CharacterController>().playerNumber = 1;
            playersInput.Add(playerInput);

            spriteRenderers[1].color = Color.white; 
            animators[1].SetBool("isMovingDown", true);

            //Trigger Animation before start
            StartCoroutine(StartLevelAfterDelay(1.5f));

        }

        playersConnected++;
    }

    IEnumerator StartLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Level-Clover");
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayersManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<PlayerInputManager>().JoinPlayer();
        GetComponent<PlayerInputManager>().JoinPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

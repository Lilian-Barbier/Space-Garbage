using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


/// <summary>
/// Cette classe est utilis� pour g�rer le choix des Personnages, puis la gestions PlayerInput / Device connect� � chaque joueurs.
/// Elle ne seras donc pas d�truite aux changement de sc�ne.
/// </summary>
public class PlayerCharactersManager : MonoBehaviour
{
    public List<InputDevice> playersDevice = new();
    public Dictionary<int, Color> colorsByPlayerIndex = new();

    //Todo: voir si �a vaut le coup de plutot le r�cup�rer avec un Ressource.Load()
    [SerializeField] GameObject playerPrefab;
    [SerializeField] TextMeshProUGUI text;

    PlayerInputManager inputManager;
    GameObject[] spawns;

    int playersValidate = 0;
    bool isStarted = false;

    private void Start()
    {
        spawns = GameObject.FindGameObjectsWithTag("Spawns");
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        inputManager = GetComponent<PlayerInputManager>();
    }

    private void OnEnable()
    {
        inputManager.onPlayerJoined += ctx => OnPlayerJoined(ctx);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnDisable()
    {
        inputManager.onPlayerJoined -= OnPlayerJoined;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #region Player Joined and Character Selection
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        if (!isStarted)
        {
            playersDevice.Add(playerInput.GetDevice<InputDevice>());
            playerInput.transform.position = spawns[playerInput.playerIndex].transform.position;
            playerInput.GetComponent<ChooseCharacterIndividual>().manager = this;

            text.text = "Press any button of another controller (keyboard or gamepad) to join.\n Or press Start / Enter to play alone";
        }
    }

    public void PlayerValidate(int playerIndex, Color playerColor)
    {
        colorsByPlayerIndex.Add(playerIndex, playerColor);
        playersValidate++;
        if (playersValidate == playersDevice.Count)
        {
            isStarted = true;

            //Trigger Animation before start
            StartCoroutine(StartLevelAfterDelay());
        }
    }

    IEnumerator StartLevelAfterDelay()
    {
        text.text = "";

        //A partir du moment ou on quitte la scene de jeu, on passe sur une gestion manuelle pour �viter que de nouveau joueur se connecte.
        //On instanciera des playerPrefab
        inputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
        inputManager.playerPrefab = playerPrefab;

        yield return new WaitForSeconds(1f);
        GameObject.FindGameObjectWithTag("LevelLoader").GetComponentInChildren<Animator>().SetTrigger("End");

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("ChooseLvl");
    }
    #endregion 

    #region On load level character and playerInput management

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Todo : voir pour trouver mieux pour filtrer sur uniquement les levels ou l'on doit instancier des joueurs
        if (SceneManager.GetActiveScene().name.Contains("Level"))
        {
            spawns = GameObject.FindGameObjectsWithTag("Spawns");
            foreach (InputDevice device in playersDevice)
            {
                //On instancie un nouveau prefab du joueur li� au bon controller.
                var player = inputManager.JoinPlayer(pairWithDevice: device);
                player.transform.position = spawns[player.playerIndex].transform.position;
                player.GetComponent<SpriteRenderer>().color = colorsByPlayerIndex[player.playerIndex];
            }
        }
    }
    #endregion

}

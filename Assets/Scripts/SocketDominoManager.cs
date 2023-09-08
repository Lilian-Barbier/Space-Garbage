using System;
using System.Collections.Generic;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;


public class SocketDominoManager : MonoBehaviour
{
    public SocketIOUnity socket;
    [SerializeField] private GameObject distantPlayerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        //TODO: check the Uri if Valid.
        var uri = new Uri("http://localhost:11100/");
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"token", "UNITY" }
                }
            ,
            EIO = 4
            ,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
        socket.JsonSerializer = new NewtonsoftJsonSerializer();

        ///// reserved socketio events
        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("socket.OnConnected");
        };
        socket.OnPing += (sender, e) =>
        {
            Debug.Log("Ping");
        };
        socket.OnPong += (sender, e) =>
        {
            Debug.Log("Pong: " + e.TotalMilliseconds);
        };
        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("disconnect: " + e);
        };
        socket.OnReconnectAttempt += (sender, e) =>
        {
            Debug.Log($"{DateTime.Now} Reconnecting: attempt = {e}");
        };
        ////

        Debug.Log("Connecting...");
        socket.Connect();

        socket.OnAnyInUnityThread((name, response) =>
        {
            Debug.Log("Received On " + name + " : " + response + "\n");
        });

        socket.On("newPlayerConnected", param =>
        {
            var instance = Instantiate(distantPlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            instance.GetComponent<DistantCharacterController>().socketId = param.GetValue<PlayerInformation>().id;
        });
    }

    public void EmitTest()
    {
        //string eventName = EventNameTxt.text.Trim().Length < 1 ? "hello" : EventNameTxt.text;
        //string txt = DataTxt.text;
        //if (!IsJSON(txt))
        //{
        //    socket.Emit(eventName, txt);
        //}
        //else
        //{
        //    socket.EmitStringAsJSON(eventName, txt);
        //}
    }

    public static bool IsJSON(string str)
    {
        if (string.IsNullOrWhiteSpace(str)) { return false; }
        str = str.Trim();
        if ((str.StartsWith("{") && str.EndsWith("}")) || //For object
            (str.StartsWith("[") && str.EndsWith("]"))) //For array
        {
            try
            {
                var obj = JToken.Parse(str);
                return true;
            }catch (Exception ex) //some other exception
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void Move(Vector2 position)
    {
        PlayerInformation data = new PlayerInformation(null, "player1", position.x, position.y);
        socket.Emit("move", data);
    }



    // our test class
    [System.Serializable]
    class TestClass
    {
        public string[] arr;

        public TestClass(string[] arr)
        {
            this.arr = arr;
        }
    }

    [System.Serializable]
    public class PlayerInformation
    {
        public string id;
        public string username;
        public float x;
        public float y;


        public PlayerInformation(string id, string username, float x, float y)
        {
            this.id = id;
            this.username = username;
            this.x = x;
            this.y = y;
        }
    }

}
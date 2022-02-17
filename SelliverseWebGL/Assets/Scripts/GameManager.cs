using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NativeWebSocket;
using System.Text;

public enum GameState
{
    Lobby,
    Joining,
    InGame,
    Dead
}

public class GameManager : MonoBehaviour
{

    class RootMessage
    {
        public string type;
    }

    class WelcomeMessage : RootMessage
    {
        public bool isWelcome;
    }

    class EnterMessage
    {
        public string type = "enter";

        public string name;
    }

    public GameState state;

    WebSocket websocket;
    InputField nameField;
    

    // Start is called before the first frame update
    async void Start()
    {
        this.state = GameState.Lobby;
        nameField = GameObject.Find("NameField").GetComponent<InputField>();

        websocket = new WebSocket("ws://localhost:5000");
        // websocketConnection = this.GetComponent<WebSocketConnection>();
        // websocketConnection = new WebSocketConnection();
        // websocketConnection.Start();
        // websocketConnection.AddHandler(handleIncomingMessage);
        
        // {
        //     // Reading a plain text message
        //     var message = System.Text.Encoding.UTF8.GetString(bytes);
        //     Debug.Log("Received OnMessage! (" + bytes.Length + " bytes) " + message);
        // };
        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
            
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += HandleMessage;

        await websocket.Connect();

    }

   

    // Update is called once per frame
    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }


    async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    public void Join()
    {
        Debug.Log("Hello " + nameField.text);
        this.state = GameState.Joining;

        var enterMsg = new EnterMessage()
        {
            name = nameField.text
        };

        var data = JsonUtility.ToJson(enterMsg);

        EmitMessage(data);
    }

    public async void EmitMessage(string msg)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText(msg);
        }
    }

    public void HandleMessage(byte[] data)
    {
        var json = Encoding.UTF8.GetString(data);
    
        var rootmsg = JsonUtility.FromJson<RootMessage>(json);
    
        Debug.Log("Got a '" + rootmsg.type + "' from the server");

        switch (rootmsg.type)
        {
            case "welcome":
                HandleWelcome(json);
                break;
            default:
                break;
        }
    }


    public void HandleWelcome(string json)
    {
        var welcomeMsg = JsonUtility.FromJson<WelcomeMessage>(json);

        if(welcomeMsg.isWelcome)
        {
            this.state = GameState.InGame;
            Debug.Log("Welcome to the game!");
        }
        else
        {
            Debug.Log("Already a player with that name");
        }
    }
}

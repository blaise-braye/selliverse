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

        public ChatMessage[] LastMessages;
    }

    class EnterMessage
    {
        public string type = "enter";

        public string name;
    }

    public bool UseLocal = true;

    public GameState state;

    WebSocket websocket;
    InputField nameField;

    public ChatController chatController;

    // Start is called before the first frame update
    async void Start()
    {
        this.state = GameState.Lobby;
        nameField = GameObject.Find("NameField").GetComponent<InputField>();
        chatController = GameObject.Find("HUD").GetComponent<ChatController>();

        websocket = new WebSocket(UseLocal ? "wss://localhost:5001" : "wss://selliverse.azurewebsites.net/");
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

        // var data = JsonUtility.ToJson(enterMsg);

        EmitMessage(enterMsg);
    }

    public async void EmitMessage(object msg)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText(JsonUtility.ToJson(msg));
        }
    }

    public void HandleMessage(byte[] data)
    {
        var json = Encoding.UTF8.GetString(data);
        Debug.Log(json);

        var rootmsg = JsonUtility.FromJson<RootMessage>(json);

        Debug.Log("Got a '" + rootmsg.type + "' from the server");

        switch (rootmsg.type)
        {
            case "welcome":
                HandleWelcome(json);
                break;
            case "chat":
                HandleChat(json);
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
            var lobby = GameObject.Find("Lobby");
            lobby.SetActive(false);
            foreach(var message in welcomeMsg.LastMessages)
            {
                chatController.AddChat(message.name, message.content);
            }
        }
        else
        {
            Debug.Log("Already a player with that name");
        }
    }

    class ChatMessage : RootMessage
    {
        public string name;

        public string content;
    }

    public void HandleChat(string json)
    {
        var chatMsg = JsonUtility.FromJson<ChatMessage>(json);

        chatController.AddChat(chatMsg.name, chatMsg.content);
    }

}

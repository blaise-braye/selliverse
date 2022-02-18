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

        public string id;
    }

    public bool UseLocal = true;

    public GameState state;

    WebSocket websocket;
    InputField nameField;

    Dictionary<string, GameObject> players;

    GameObject selliFab;
    public ChatController chatController;

    // Start is called before the first frame update
    async void Start()
    {
        this.state = GameState.Lobby;
        nameField = GameObject.Find("NameField").GetComponent<InputField>();
        chatController = GameObject.Find("HUD").GetComponent<ChatController>();
        selliFab = GameObject.Find("SelliFab");
        var uri = UseLocal ? "wss://localhost:5001" : "wss://selliverse.azurewebsites.net/";
        websocket = new WebSocket(uri);
        players = new Dictionary<string, GameObject>();

        Debug.Log("connecting to " + uri);

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

        Debug.Log("Got a '" + rootmsg.type + "' from the server : " + json);
        switch (rootmsg.type)
        {
            case "welcome":
                HandleWelcome(json);
                break;
            case "chat":
                HandleChat(json);
                break;
            case "movement":
                HandleMovement(json);
                break;
            case "entered":
                HandleEntered(json);
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
        }
        else
        {
            Debug.Log("Already a player with that name");
        }
    }

    public void HandleMovement(string json)
    {
        Debug.Log("Got some movement " + json);
        var moveMsg = JsonUtility.FromJson<MovementMessage>(json);

        if(this.players.TryGetValue(moveMsg.id, out GameObject go))
        {
            var location = new Vector3(
                float.Parse(moveMsg.x),
                float.Parse(moveMsg.y),
                float.Parse(moveMsg.z)
            );

            go.transform.position = location;
        }
    }

    public void HandleEntered(string json)
    {
        Debug.Log("Someone entered " + json);
        var enterMsg = JsonUtility.FromJson<EnterMessage>(json);

        GameObject childGameObject = Instantiate(selliFab, new Vector3(-55f, 5f, -50f), Quaternion.identity);
        this.players.Add(enterMsg.id, childGameObject);
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

    class MovementMessage : RootMessage
    {
        public string id;
        public string x;

        public string y;

        public string z;
    }

}

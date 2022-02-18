using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NativeWebSocket;
using System.Text;
using System.Globalization;

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

    class MoveMessage : RootMessage
    {
        public string x;
        public string y;
        public string z;
    }

    public bool UseLocal = true;

    public GameState state;

    WebSocket websocket;
    InputField nameField;

    Dictionary<string, GameObject> players;

    GameObject selliFab;
    public ChatController chatController;
    PlayerMovement playerMovement;

    // Start is called before the first frame update
    async void Start()
    {
        this.state = GameState.Lobby;
        nameField = GameObject.Find("NameField").GetComponent<InputField>();
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
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
        // Debug.Log(json);

        var rootmsg = JsonUtility.FromJson<RootMessage>(json);

        // 
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
            case "rotation":
                HandleRotation(json);
                break;
            case "left":
                HandleLeft(json);
                break;
            case "move":
                HandleMove(json);
                break;
            default:
                Debug.Log("Got a '" + rootmsg.type + "' from the server : " + json);
                break;
        }
    }


    public void HandleWelcome(string json)
    {
        var welcomeMsg = JsonUtility.FromJson<WelcomeMessage>(json);

        var lobby = GameObject.Find("Lobby");
        if(welcomeMsg.isWelcome)
        {
            this.state = GameState.InGame;
            Debug.Log("Welcome to the game!");
            lobby.SetActive(false);
        }
        else
        {
            this.state = GameState.Lobby;
            Debug.Log("Already a player with that name");
            lobby.SetActive(true);
        }
    }

    public void HandleLeft(string json)
    {
        var leftMsg = JsonUtility.FromJson<LeftMessage>(json);

        if(this.players.TryGetValue(leftMsg.id, out GameObject go))
        {
            this.players.Remove(leftMsg.id);
            Destroy(go);
        }
    }

    public void HandleMovement(string json)
    {
        var moveMsg = JsonUtility.FromJson<MovementMessage>(json);

        if(this.players.TryGetValue(moveMsg.id, out GameObject go))
        {
            var location = new Vector3(
                float.Parse(moveMsg.x, CultureInfo.InvariantCulture),
                float.Parse(moveMsg.y, CultureInfo.InvariantCulture),
                float.Parse(moveMsg.z, CultureInfo.InvariantCulture)
            );

            go.transform.position = location;
        }
    }

    
    public void HandleRotation(string json)
    {
        var rotMsg = JsonUtility.FromJson<RotationMessage>(json);
        if(this.players.TryGetValue(rotMsg.id, out GameObject go))
        {
            go.gameObject.transform.rotation = Quaternion.Euler(float.Parse(rotMsg.x, CultureInfo.InvariantCulture), 0.0f, 0.0f);
            // go.gameObject.transform.Rotate(Vector3.up * ((float.Parse(rotMsg.x, CultureInfo.InvariantCulture) + (Mathf.PI / 4.0f)) ));
        }
    }


    public void HandleEntered(string json)
    {
        Debug.Log("Someone entered " + json);
        var enterMsg = JsonUtility.FromJson<EnterMessage>(json);

        GameObject childGameObject = Instantiate(selliFab, new Vector3(-55f, 5f, -50f), Quaternion.identity);
        var text = childGameObject.GetComponentInChildren<TextMesh>();
        text.text = enterMsg.name;
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

    public void HandleMove(string json)
    {
        var chatMsg = JsonUtility.FromJson<MoveMessage>(json);

        var location = new Vector3(
                float.Parse(chatMsg.x, CultureInfo.InvariantCulture),
                float.Parse(chatMsg.y, CultureInfo.InvariantCulture),
                float.Parse(chatMsg.z, CultureInfo.InvariantCulture)
            );

        playerMovement.Teleport(location);

        Debug.Log("Move Command " + json);
    }

    class MovementMessage : RootMessage
    {
        public string id;
        public string x;

        public string y;

        public string z;
    }

    class RotationMessage : RootMessage
    {
        public string id;

        public string x;
    }

    class LeftMessage
    {
        public string type = "left";

        public string id;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Lobby,
    Joining,
    InGame,
    Dead
}

public class GameManager : MonoBehaviour
{

    class EnterMessage
    {
        public string type = "enter";

        public string name;
    }

    public GameState state;

    InputField nameField;
    WebSocketConnection websocketConnection;

    // Start is called before the first frame update
    void Start()
    {
        this.state = GameState.Lobby;
        nameField = GameObject.Find("NameField").GetComponent<InputField>();

        websocketConnection = this.GetComponent<WebSocketConnection>();
        // websocketConnection = new WebSocketConnection();
        // websocketConnection.Start();
        // websocketConnection.AddHandler(handleIncomingMessage);
        
        // {
        //     // Reading a plain text message
        //     var message = System.Text.Encoding.UTF8.GetString(bytes);
        //     Debug.Log("Received OnMessage! (" + bytes.Length + " bytes) " + message);
        // };
    }

    public void EmitEvent(string msg)
    {
        websocketConnection.Send(msg);
    }

    void handleIncomingMessage(byte[] bytes)
    {
        Debug.Log("Got a message!");
    }

    // Update is called once per frame
    void Update()
    {
        
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

        websocketConnection.Send(data);
    }
}

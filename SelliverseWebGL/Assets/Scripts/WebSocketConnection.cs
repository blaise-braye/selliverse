using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

public class WebSocketConnection : MonoBehaviour
{
    WebSocket websocket;

    bool isOpen = false;

    // Start is called before the first frame update
    public async void Start()
    {
        websocket = new WebSocket("ws://localhost:5000");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            isOpen = true;
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
            
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            isOpen = false ;
        };

        // websocket.OnMessage += (bytes) =>
        // {
        //     Debug.Log("Hello got something");
        //     // Reading a plain text message
        //     var message = System.Text.Encoding.UTF8.GetString(bytes);
        //     Debug.Log("Received OnMessage! (" + bytes.Length + " bytes) " + message);
        // };


        await websocket.Connect();


    }

    // Update is called once per frame
    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    public void AddHandler(WebSocketMessageEventHandler handler)
    {
        websocket.OnMessage += handler;
        Debug.Log("Added handler!");
    }

    async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    public async void Send(string msg)
    {
        if (isOpen)
        {
            await websocket.SendText(msg);
        }
    }
}

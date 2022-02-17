using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

public class WebSocketConnection : MonoBehaviour
{
    WebSocket websocket;

    bool isOpen = false;

    // Start is called before the first frame update
    async void Start()
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


        await websocket.Connect();


    }

    // Update is called once per frame
    void Update()
    {

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

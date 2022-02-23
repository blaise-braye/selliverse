using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NativeWebSocket;
using UnityEngine;

namespace Assets.Scripts
{
    public class WebSocketConnection
    {
        WebSocket websocket;
        private Task websocketConnection;
        private bool isClosing;

        private string uri;
        public static WebSocketConnection Instance { get; } = new WebSocketConnection();

        private readonly List<Action<RootMessage, string>> MessageSubscriber = new List<Action<RootMessage, string>>();

        private bool IsConnected => websocket?.State == WebSocketState.Open;
    
        public void Init(bool useLocal)
        {
            if (websocketConnection != null)
            {
                throw new InvalidOperationException();
            }

            this.uri = useLocal ? "wss://localhost:5001" : "wss://selliverse.azurewebsites.net/";
            Debug.Log("connecting to " + uri);
        
            var tmpWebsocket = CreateWebsocket();
            websocket = tmpWebsocket;
            websocketConnection = tmpWebsocket.Connect();
        }

        private WebSocket CreateWebsocket()
        {
            var tmpWebsocket = new WebSocket(uri);

            tmpWebsocket.OnOpen += () => Debug.Log("Connection open!");
            tmpWebsocket.OnError += (e) => Debug.Log("Error! " + e);
            tmpWebsocket.OnClose += (e) => ForceReconnect();

            tmpWebsocket.OnMessage += OnMessageReceived;
            return tmpWebsocket;
        }

        private void ForceReconnect()
        {
            if (isClosing) return;

            var tmpWebsocket = CreateWebsocket();
            websocket = tmpWebsocket;
            websocketConnection = tmpWebsocket.Connect();
        }

        public void AddMessageSubscriber(Action<RootMessage, string> subscription)
        {
            MessageSubscriber.Add(subscription);
        }

        private void OnMessageReceived(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);
            Debug.Log($"OnMessageReceived {json}");
            var rootMsg = JsonUtility.FromJson<RootMessage>(json);
            foreach (var subscriber in MessageSubscriber)
            {
                subscriber(rootMsg, json);
            }
        }

        public async void SendMessage(object msg)
        {
            if (!IsConnected) return;
            Debug.Log($"SendMessage {JsonUtility.ToJson(msg)}");
            await websocket.SendText(JsonUtility.ToJson(msg));
        }

        // Update is called once per frame
        public void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            websocket?.DispatchMessageQueue();
#endif
        }

        public async void OnApplicationQuit()
        {
            isClosing = true;
            if (!IsConnected) return;
            await websocket.Close();
            websocket = null;
            websocketConnection = null;
        }
    }
}

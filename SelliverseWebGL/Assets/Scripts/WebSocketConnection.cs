using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeWebSocket;
using UnityEngine;

namespace Assets.Scripts
{
    public class WebSocketConnection
    {
        WebSocket websocket;
        private bool isClosing;

        private string uri;

        private static readonly Lazy<WebSocketConnection> LazyInstance =
            new Lazy<WebSocketConnection>(() => new WebSocketConnection());

        public static WebSocketConnection Instance => LazyInstance.Value;

        private Action<RootMessage> messageHandler;

        private bool IsConnected => websocket?.State == WebSocketState.Open;

        public async Task Connect(bool useLocal, Action<RootMessage> newMessageHandler)
        {
            this.messageHandler = newMessageHandler;
            this.uri = useLocal ? "wss://localhost:5001" : "wss://selliverse.azurewebsites.net/";
            Debug.Log("connecting to " + uri);

            websocket = CreateWebsocket();

            var backoff = new ExponentialBackoff(200, 2000);

            while (!isClosing)
            {
                if (backoff.Retries > 10)
                {
                    backoff.Reset();
                }

                try
                {
                    await websocket.Connect();
                }
                catch(Exception exn)
                {
                    Debug.Log("Connection failed : " + exn.Message);
                }

                if (isClosing) continue;
                Debug.Log($"Retrying to Connect in {backoff.GetNextDelay()} msec.");
                await backoff.Delay();
                if (isClosing) continue;
                Debug.Log($"Retrying to Connect");
            }
        }

        private WebSocket CreateWebsocket()
        {
            var tmpWebsocket = new WebSocket(uri);

            tmpWebsocket.OnOpen += () => OnMessageReceived(new ConnectionStateMessage(true));
            tmpWebsocket.OnError += (e) => Debug.Log("WebSocket Error! " + e);
            tmpWebsocket.OnClose += (e) => OnMessageReceived(new ConnectionStateMessage(false));
            tmpWebsocket.OnMessage += OnMessageReceived;
            return tmpWebsocket;
        }

        private static readonly Dictionary<string, Type> MessageTypeToNetTypeMappings = typeof(RootMessage).Assembly.GetTypes().Where(typeof(RootMessage).IsAssignableFrom)
            .Where(t => t != typeof(RootMessage))
            .Select(Activator.CreateInstance)
            .Cast<RootMessage>()
            .ToDictionary(msg => msg.type, msg => msg.GetType());

        private void OnMessageReceived(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);
            var rootMsg = JsonUtility.FromJson<RootMessage>(json);

            if (MessageTypeToNetTypeMappings.TryGetValue(rootMsg.type, out var msgConcreteType))
            {
                var concreteMessage = (RootMessage) JsonUtility.FromJson(json, msgConcreteType);
                OnMessageReceived(concreteMessage);
            }
            else
            {
                Debug.Log($"Message type unknown {rootMsg.type}");
            }
        }

        private void OnMessageReceived(RootMessage message)
        {
            messageHandler?.Invoke(message);
        }

        public async void SendMessage(object msg)
        {
            if (!IsConnected) return;
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
        }
    }
}

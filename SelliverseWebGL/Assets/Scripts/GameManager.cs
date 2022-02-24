using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public bool UseLocal = true;

        public GameState state;
        
        InputField nameField;
        

        private string lastWelcomedName;

        Dictionary<string, GameObject> players;

        GameObject selliFab;
        public ChatController chatController;
        PlayerMovement playerMovement;
        private GameObject lobby;
        private Text connectionLostStateText;

        // Start is called before the first frame update
        async void Start()
        {
            this.state = GameState.Lobby;
            nameField = GameObject.Find("NameField").GetComponent<InputField>();
            connectionLostStateText = GameObject.Find("ConnectionStateText").GetComponent<Text>();
            playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
            chatController = GameObject.Find("HUD").GetComponent<ChatController>();
            selliFab = GameObject.Find("SelliFab");
            lobby = GameObject.Find("Lobby");
            players = new Dictionary<string, GameObject>();
            
            await WebSocketConnection.Instance.Connect(this.UseLocal, HandleMessage);
        }
        
        // Update is called once per frame
        void Update()
        {
            WebSocketConnection.Instance.Update();
        }
        
        void OnApplicationQuit()
        {
            WebSocketConnection.Instance.OnApplicationQuit();
        }

        public void Join()
        {
            Debug.Log("Hello " + nameField.text);
            this.state = GameState.Joining;

            var enterMsg = new EnterMessage()
            {
                name = nameField.text
            };
            
            WebSocketConnection.Instance.SendMessage(enterMsg);
        }
        
        private void HandleMessage(RootMessage rootMessage)
        {
            switch (rootMessage)
            {
                case ConnectionStateMessage msg:
                    HandleConnectionState(msg);
                    break;
                case WelcomeMessage msg:
                    HandleWelcome(msg);
                    break;
                case ChatMessage msg:
                    HandleChat(msg);
                    break;
                case MovementMessage msg:
                    HandleMovement(msg);
                    break;
                case EnterMessage msg:
                    HandleEntered(msg);
                    break;
                case RotationMessage msg:
                    HandleRotation(msg);
                    break;
                case LeftMessage msg:
                    HandleLeft(msg);
                    break;
                case MoveMessage msg:
                    HandleMove(msg);
                    break;
                default:
                    Debug.Log("Got a '" + rootMessage.type + "' from the server");
                    break;
            }
        }
        
        private void HandleConnectionState(ConnectionStateMessage msg)
        {
            if (msg.IsConnected)
            {
                connectionLostStateText.text = "";
                if (!string.IsNullOrEmpty(lastWelcomedName))
                {
                    Join();
                }
            }
            else
            {
                connectionLostStateText.color = Color.red;
                connectionLostStateText.text = "Disconnected";
            }
        }

        public void HandleWelcome(WelcomeMessage welcomeMsg)
        {
            if (welcomeMsg.isWelcome)
            {
                this.state = GameState.InGame;
                Debug.Log("Welcome to the game!");
                lobby.SetActive(false);
                lastWelcomedName = nameField.text;
            }
            else
            {
                this.state = GameState.Lobby;
                Debug.Log("Already a player with that name");
                lobby.SetActive(true);
            }
        }

        public void HandleLeft(LeftMessage leftMsg)
        {
            if (this.players.TryGetValue(leftMsg.id, out GameObject go))
            {
                this.players.Remove(leftMsg.id);
                Destroy(go);
            }
        }

        public void HandleMovement(MovementMessage moveMsg)
        {
            if (this.players.TryGetValue(moveMsg.id, out GameObject go))
            {
                var location = new Vector3(
                    float.Parse(moveMsg.x, CultureInfo.InvariantCulture),
                    float.Parse(moveMsg.y, CultureInfo.InvariantCulture),
                    float.Parse(moveMsg.z, CultureInfo.InvariantCulture)
                );

                go.transform.position = location;
            }
        }
        
        public void HandleRotation(RotationMessage rotMsg)
        {
            if (this.players.TryGetValue(rotMsg.id, out GameObject go))
            {
                go.gameObject.transform.rotation =
                    Quaternion.Euler(
                        0.0f,
                        float.Parse(rotMsg.x, CultureInfo.InvariantCulture) + 90f,
                        0.0f);
                // go.gameObject.transform.Rotate(Vector3.up * ((float.Parse(rotMsg.x, CultureInfo.InvariantCulture) + (Mathf.PI / 4.0f)) ));
            }
        }
        
        public void HandleEntered(EnterMessage enterMsg)
        {
            GameObject childGameObject = Instantiate(selliFab, new Vector3(-55f, 5f, -50f), Quaternion.identity);
            var text = childGameObject.GetComponentInChildren<TextMesh>();
            text.text = enterMsg.name;
            this.players.Add(enterMsg.id, childGameObject);
        }

        public void HandleChat(ChatMessage chatMsg)
        {
            chatController.AddChat(chatMsg.name, chatMsg.content);
        }

        public void HandleMove(MoveMessage chatMsg)
        {
            var location = new Vector3(
                float.Parse(chatMsg.x, CultureInfo.InvariantCulture),
                float.Parse(chatMsg.y, CultureInfo.InvariantCulture),
                float.Parse(chatMsg.z, CultureInfo.InvariantCulture)
            );

            playerMovement.Teleport(location);
        }

    }
}
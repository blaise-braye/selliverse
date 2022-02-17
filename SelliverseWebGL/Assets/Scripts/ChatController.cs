using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChatController : MonoBehaviour
{

    class ChatMessage
    {
        public string type = "chat";

        public string content;
    }

    public bool isChatting = false;
    GameManager gameManager;

    EventSystem eventSystem;

    GameObject canvas;
    InputField chatBox;
    Text chatText;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game").GetComponent<GameManager>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        canvas = GameObject.Find("HudCanvas");
        chatBox = GameObject.Find("ChatBox").GetComponent<InputField>();
        chatText = GameObject.Find("ChatText").GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.state == GameState.InGame)
        {
            canvas.SetActive(true);
            // this.gameObject..gameObject.SetActive(true);
            
            if(!isChatting)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    Debug.Log("RETURN");
                    eventSystem.SetSelectedGameObject(chatBox.gameObject);
                    isChatting = true;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Return) && chatBox.text.Length > 0)
                {
                    Debug.Log("Chatting :" + chatBox.text);
                    gameManager.EmitMessage(new ChatMessage()
                        {
                            content = chatBox.text
                        });
                    chatBox.text = "";
                    isChatting = false;
                }
            }
            

        }
        
        else
        {
            canvas.SetActive(false);
            // this.gameObject.GetComponent<Canvas>().gameObject.SetActive(false);
        }
    }

    public void AddChat(string name, string content)
    {
        this.chatText.text +=  "\r\n" + name + ": " + content;
    }
}

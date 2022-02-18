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

    class CommandMessage
    {
        public string type = "command";

        public string content;
    }

    public bool isChatting = false;


    GameManager gameManager;

    EventSystem eventSystem;

    GameObject canvas;
    InputField chatBox;
    Text chatText;

    Queue<string> chatlines;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game").GetComponent<GameManager>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        canvas = GameObject.Find("HudCanvas");
        chatBox = GameObject.Find("ChatBox").GetComponent<InputField>();
        chatText = GameObject.Find("ChatText").GetComponent<Text>();
        chatlines = new Queue<string>();
        chatlines.Enqueue("Welcome to the Selliverse!");
        this.chatText.text = "Welcome to the Selliverse!";
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
                    eventSystem.SetSelectedGameObject(chatBox.gameObject);
                    isChatting = true;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if(chatBox.text.Length > 0)
                    {
                        if(chatBox.text.Equals("/quit", System.StringComparison.OrdinalIgnoreCase))
                        {
                            Debug.Log("Quitting");
                            Application.Quit();
                        }

                        Debug.Log("Chatting :" + chatBox.text);

                        //if (chatBox.text.StartsWith("/"))
                        //{
                        //    Debug.Log("Command :" + chatBox.text);
                        //    gameManager.EmitMessage(new CommandMessage()
                        //    {
                        //        content = chatBox.text.Substring(1)
                        //    });
                        //}
                        //else
                        //{
                        //    gameManager.EmitMessage(new ChatMessage()
                        //    {
                        //        content = chatBox.text
                        //    });
                        //}
                        gameManager.EmitMessage(new ChatMessage()
                        {
                            content = chatBox.text
                        });
                        chatBox.text = "";
                        isChatting = false;
                    }
                }
                if(Input.GetKeyDown(KeyCode.Escape))
                {
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
        this.chatlines.Enqueue(name + ": " + content);
        while(this.chatlines.Count > 4)
        {
            this.chatlines.Dequeue();
        }
        // this.chatText.text +=  "\r\n" + name + ": " + content;
        this.chatText.text = "";
        foreach(var line in this.chatlines)
        {
            this.chatText.text += line + "\r\n";
        }
    }
}

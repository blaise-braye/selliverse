using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LobbyController : MonoBehaviour
{
    GameManager gameManager;

    EventSystem eventSystem;

    InputField loginInputField;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game").GetComponent<GameManager>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        loginInputField = GameObject.Find("NameField").GetComponent<InputField>();

        eventSystem.SetSelectedGameObject(loginInputField.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(gameManager.state);
        if(gameManager.state == GameState.Lobby)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                gameManager.Join();
            }
        }
    }
}

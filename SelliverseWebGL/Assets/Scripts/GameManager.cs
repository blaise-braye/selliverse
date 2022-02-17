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
    public GameState state;

    InputField nameField;

    // Start is called before the first frame update
    void Start()
    {
        this.state = GameState.Lobby;
        nameField = GameObject.Find("NameField").GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void Join()
    {
        Debug.Log("Hello " + nameField.text);
    }
}

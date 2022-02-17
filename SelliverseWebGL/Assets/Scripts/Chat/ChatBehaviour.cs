using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject chatUI = null;
    [SerializeField] private TMP_Text chatText = null;
    [SerializeField] private TMP_InputField inputField = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleNewMessage(string message)
    {
        chatText.text += "\n" + message;
    }
}

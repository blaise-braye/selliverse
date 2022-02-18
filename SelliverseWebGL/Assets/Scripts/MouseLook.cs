using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    public Transform playerBody;


    GameManager gameManager;

    float xRotation = 0f;

    bool focus = false;

    

    // Start is called before the first frame update
    void Start()
    {
        var game = GameObject.Find("Game");
        gameManager = game.GetComponent<GameManager>();

        
        
    }

    void OnApplicationFocus(bool focused)
    {
        // if (gameManager.state == GameState.InGame)
        {
             // focus = focused;
        } 
    }

    // Update is called once per frame
    void Update()
    {

        if (gameManager.state == GameState.InGame)
        {

            Cursor.lockState = CursorLockMode.Locked;
            
            if(Cursor.lockState == CursorLockMode.Locked)
            {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
            }
        }
    }
}

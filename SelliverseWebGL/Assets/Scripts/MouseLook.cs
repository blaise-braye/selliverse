using System.Globalization;
using UnityEngine;

namespace Assets.Scripts
{
    public class MouseLook : MonoBehaviour
    {
        class RotationMessage
        {
            public string type = "rotation";

            public string x;
        }
        public float mouseSensitivity = 100f;

        public Transform playerBody;


        GameManager gameManager;

        float xRotation = 0f;

        bool focus = false;


        float lastRotPush = 0f;
        private RotationMessage lastMsg;

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
                    var veccy =  playerBody.transform.rotation.eulerAngles;


                    lastRotPush = xRotation;
                    var msg = new RotationMessage()
                    {
                        x = veccy.y.ToString(CultureInfo.InvariantCulture),
                        // y = veccy.y.ToString(CultureInfo.InvariantCulture),
                        //z = veccy.z.ToString(CultureInfo.InvariantCulture)
                    };

                    if (msg.x != lastMsg?.x)
                    {
                        lastMsg = msg;
                        WebSocketConnection.Instance.SendMessage(msg);
                    }
                }
            }
        }
    }
}

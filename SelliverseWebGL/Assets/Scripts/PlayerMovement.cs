using System.Globalization;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerMovement : MonoBehaviour
    {

        class MovementMessage
        {
            public string type = "movement";

            public string x;

            public string y;

            public string z;
        }

    
        public CharacterController controller;

        public Transform groundCheck;

        public float groundDistance = 0.4f;

        public LayerMask groundMask;

        public float speed = 12f;

        public float gravity = -9.81f;

        public float jumpHeight = 3f;

        Vector3 velocity;
        bool isGrounded;

        GameManager gameManager;

        bool teleport = false;
        Vector3 newPosition = Vector3.zero;

        void Start()
        {
            var game = GameObject.Find("Game");
            gameManager = game.GetComponent<GameManager>();
            Debug.Log("Stgarting player movement");
        }
    
        public void Teleport(Vector3 destination)
        {
            teleport = true;
            newPosition = destination;
        }

        private void FixedUpdate()
        {
            if (teleport)
            {
                controller.transform.position = newPosition;
                teleport = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            var oldPosition = controller.transform.position;

            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
            if(isGrounded && velocity.y < 0)
            {
                velocity.y = -2.0f;
            }

            float x = gameManager.chatController.isChatting ? 0f :  Input.GetAxis("Horizontal");
            float z = gameManager.chatController.isChatting ? 0f : Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * speed * Time.deltaTime);


            if(!gameManager.chatController.isChatting && Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
            
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }


            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);


            if(Vector3.Distance(oldPosition, controller.transform.position) > 0.01f)
            {
                var msg = new MovementMessage()
                {
                    x = controller.transform.position.x.ToString(CultureInfo.InvariantCulture),
                    y = controller.transform.position.y.ToString(CultureInfo.InvariantCulture),
                    z = controller.transform.position.z.ToString(CultureInfo.InvariantCulture),
                };

                // var data = JsonUtility.ToJson(msg);
                WebSocketConnection.Instance.SendMessage(msg);
            }
        }
    }
}

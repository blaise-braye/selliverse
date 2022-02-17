using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    void Start()
    {
        var game = GameObject.Find("Game");
        gameManager = game.GetComponent<GameManager>();
        Debug.Log("Stgarting player movement");
    }
    

    // Update is called once per frame
    async void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2.0f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);


        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("JUMP!!");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }


        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);


        var msg = new MovementMessage()
        {
            x = controller.transform.position.x.ToString(),
            y = controller.transform.position.y.ToString(),
            z = controller.transform.position.z.ToString(),
        };

        // var data = JsonUtility.ToJson(msg);

        gameManager.EmitMessage(msg);

    }
}

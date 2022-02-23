using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
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
            if(gameManager.state == GameState.Lobby)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    gameManager.Join();
                }
            }
        }
    }
}

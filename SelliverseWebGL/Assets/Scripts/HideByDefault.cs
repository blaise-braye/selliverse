using UnityEngine;

namespace Assets.Scripts
{
    public class HideByDefault : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            this.gameObject.SetActive(false);
        }

        void Update()
        {

        }
    }
}

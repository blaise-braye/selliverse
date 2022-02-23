using UnityEngine;

namespace Assets.Scripts
{
    public class LookAtCamera : MonoBehaviour
    {
        public Transform textMeshTransform;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            textMeshTransform.rotation = Quaternion.LookRotation(textMeshTransform.position - Camera.main.transform.position);
        }
    }
}

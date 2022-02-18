using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Regional : MonoBehaviour
{
    public string Name;

    Text locationText;
    // Start is called before the first frame update
    void Start()
    {
        locationText = GameObject.Find("LocationText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
        // Debug.Log("Colliding " + locationText);
    }

    void OnTriggerStay(Collider collider)
    {
        locationText.text = Name;
    }

    void OnTriggerExit(Collider collider)
    {
        locationText.text = "";
    }
}

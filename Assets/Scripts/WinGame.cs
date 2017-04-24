using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGame : KeyedDoor
{

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        // If the player has the required key to leave the map, show the text to escape
        if (other.CompareTag("Player") && HasRequiredKeyToOpenDoor())
        {
            GameManager.instance.eEscape.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // If the player has the key to leave the map and the player presses E, take the text off the screen then go to the win scene
        if (other.CompareTag("Player") && HasRequiredKeyToOpenDoor() && Input.GetKeyDown(KeyCode.E))
        {
            OnTriggerExit(other);
            SceneManager.LoadScene("Win");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Hide the text when they leave the trigger zone
        if (other.CompareTag("Player"))
        {
            GameManager.instance.eEscape.SetActive(false);
        }
    }
}

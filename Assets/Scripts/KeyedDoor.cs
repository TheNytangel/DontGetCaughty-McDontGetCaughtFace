using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyedDoor : MonoBehaviour
{
    public string requiredKey;

    private Door door;

	// Use this for initialization
	void Start ()
    {
        // Get the door script
        door = GetComponent<Door>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        // If the player has the key and the door isn't already open, show them the text
        if(other.CompareTag("Player") && HasRequiredKeyToOpenDoor() && !door.isOpen())
        {
            GameManager.instance.eOpen.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // If the player has the key and the door isn't already open and the player presses E, open the door
        if (other.CompareTag("Player") && HasRequiredKeyToOpenDoor() && !door.isOpen() && Input.GetKeyDown(KeyCode.E))
        {
            // Open the gate/door
            door.Open();

            // Call the exit function to hide the text
            OnTriggerExit(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Remove text from screen
        if (other.CompareTag("Player"))
        {
            GameManager.instance.eOpen.SetActive(false);
        }
    }

    protected bool HasRequiredKeyToOpenDoor()
    {
        // Go through all the keys in the map and check if they are owned by the player. If one is and it's the one for this door, return true
        for(int i = 0; i < GameManager.instance.keys.Length; ++i)
        {
            Key tempKey = GameManager.instance.keys[i].GetComponent<Key>();
            if (tempKey.usedFor == requiredKey && tempKey.owned)
            {
                return true;
            }
        }

        return false;
    }
}

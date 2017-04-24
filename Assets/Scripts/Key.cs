using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Key : MonoBehaviour
{
    public bool owned;
    public string usedFor;

    private AudioSource soundEffect;

    // Use this for initialization
    void Start ()
    {
        // Get the sound effect from the parent. This is done because if we set this to inactive and try to play the sound, it doesn't play the sound.
        soundEffect = transform.parent.gameObject.GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    // Does this so that when they key is SetActive(true) (or spawned) it sets owned to false.
    // A.K.A. if it's in the world, it's not owned
    private void OnEnable()
    {
        owned = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the player is in the trigger area, show the "Press E to take key" text
        if(other.CompareTag("Player"))
        {
            // Show different text if it's the end "key"
            if (usedFor == "exit")
            {
                GameManager.instance.eText.GetComponent<Text>().text = "Press E to pick up the cat";
            }

            // Show the text on the screen
            GameManager.instance.eText.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // If the player presses E
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            // Play the sound effect
            soundEffect.Play();

            // Remove the key from the world
            gameObject.SetActive(false);

            // Do the OnTriggerExit stuff (need to call this because it doesn't get called on gameobject deactivation
            OnTriggerExit(other);

            // Own this key
            // (•_•)
            // ( •_•)>⌐■-■
            // (⌐■_■)
            owned = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the player leaves the trigger area, hide the E text
        if (other.CompareTag("Player"))
        {
            GameManager.instance.eText.SetActive(false);

            // Return text to normal if it was changed
            if (usedFor == "exit")
            {
                GameManager.instance.eText.GetComponent<Text>().text = "Press E to pick up the key";
            }
        }
    }
}

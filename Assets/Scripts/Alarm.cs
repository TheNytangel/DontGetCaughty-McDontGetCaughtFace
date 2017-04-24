using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
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
        if (other.CompareTag("Player"))
        {
            // If the player enters the warehouse and the alarm isn't already playing, play the alarm
            if (!GameManager.instance.alarm.isPlaying)
            {
                GameManager.instance.alarm.Play();
            }
        }
    }
}

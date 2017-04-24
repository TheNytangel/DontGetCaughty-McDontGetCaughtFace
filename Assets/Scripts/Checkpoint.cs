using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Vector3 position;

	// Use this for initialization
	void Start ()
    {
        position = GetComponent<Transform>().position;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        GameManager.instance.SetCheckpoint(position);
    }
}

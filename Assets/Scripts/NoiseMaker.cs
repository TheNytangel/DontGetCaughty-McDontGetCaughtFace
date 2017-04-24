using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMaker : MonoBehaviour
{
    private CharacterController playerCharacterController;

    // Use this for initialization
    void Start ()
    {
        playerCharacterController = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    public bool IsMakingNoise()
    {
        // If the player is moving, he/she is making noise
        return playerCharacterController.velocity.magnitude > 0;
    }
}

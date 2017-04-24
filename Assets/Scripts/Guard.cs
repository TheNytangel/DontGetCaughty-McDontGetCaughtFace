using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    public float visibility = 5.0F;
    public float hearingRadius = 10.0F;
    public float fieldOfView = 60.0F;
    public float timeToNextWaypoint = 10.0F;
    public float lookSpeed = 0.01F;
    public GameObject[] waypoints;
    public Door doorRequiredToBeOpenToMove = null;
    public GameObject questionMarkText = null;
    private bool doorIsOpen = false;

    private AudioSource audioSource;
    public AudioClip hello;

    private Transform tf;
    private Vector3 face;
    private Vector3 startPosition;
    private NavMeshAgent agent;

    private bool trackingPlayer = false;

	// Use this for initialization
	void Start ()
    {
        tf = GetComponent<Transform>();
        startPosition = tf.position;
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = hello;

        // If the guard doesn't have a door that's required to be open before it can move, or the door that is required to be open is open, start moving
        if (doorRequiredToBeOpenToMove == null || doorRequiredToBeOpenToMove.isOpen())
        {
            InvokeRepeating("GoSomewhere", 10.0F, timeToNextWaypoint);
        }
    }
    	
	// Update is called once per frame
	void Update ()
    {
        Debug.DrawRay(face, tf.forward * visibility, Color.blue);
        // Set the position of the face because that is what is used for the "canSee" function. Otherwise it would check from the middle of the guard's body
        // and the guard might not see the player
        face = transform.position + new Vector3(0, 0.75F, 0);
        
        // If the door isn't recognized as open, and there is a door that is required to open, check if the door has been opened. if it has, start moving
        if (!doorIsOpen && doorRequiredToBeOpenToMove != null && doorRequiredToBeOpenToMove.isOpen())
        {
            doorIsOpen = true;
            InvokeRepeating("GoSomewhere", 10.0F, timeToNextWaypoint);
        }

        if (CanSeeTarget(GameManager.instance.player))
        {
            // The guard can see the player so tell the gamemanager the player has been scene
            trackingPlayer = true;
            GameManager.instance.Seen();
        }
        else if (CanHearTarget(GameManager.instance.player))
        {
            trackingPlayer = true;

            // If the guard can hear the player, stop moving and say, "hello?"
            agent.SetDestination(tf.position);

            // Slowly rotate towards the player
            tf.rotation = Quaternion.Slerp(tf.rotation, Quaternion.LookRotation((GameManager.instance.player.transform.position - tf.position).normalized), lookSpeed);

            // Play the sound the guard makes when he hears the player
            audioSource.Play();

            // If the guard has a question mark gameobject, show it
            if(questionMarkText != null)
            {
                questionMarkText.SetActive(true);
            }
        }
        else
        {
            trackingPlayer = false;

            // If the guard has a question mark gameobject, hide it
            if (questionMarkText != null)
            {
                questionMarkText.SetActive(false);
            }
        }
	}

    void GoSomewhere()
    {
        // If the guard isn't tracking the player (i.e. he can't hear the player), move to a new location
        if (!trackingPlayer)
        {
            agent.SetDestination(waypoints[Random.Range(0, waypoints.Length)].transform.position);
        }
    }

    public void ResetGuard()
    {
        // Reset to the position the guard spawned at
        agent.SetDestination(startPosition);
        tf.position = startPosition;
    }

    public bool CanHearTarget(GameObject target)
    {
        // Check if the player is making noise (moving) and is less than the hearing radius distance away from the guard
        if(target.GetComponent<NoiseMaker>().IsMakingNoise() && Vector3.Distance(target.transform.position, tf.position) <= hearingRadius)
        {
            return true;
        }
        return false;
    }

    public bool CanSeeTarget(GameObject target)
    {
        // Get the transform component of the target since it gets used a couple times
        Transform targetTransform = target.GetComponent<Transform>();
        // Calculate the vector from the guard to the target
        Vector3 vectorToTarget = targetTransform.position - face;
        RaycastHit hit;

        // Check if the player is within the field of view and is not obstructed by an object
        if (Vector3.Angle(tf.forward, vectorToTarget) <= fieldOfView
            && Physics.Raycast(face, vectorToTarget, out hit, visibility)
            && hit.collider.gameObject == target)
        {
            return true;
        }

        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Whether or not this is a single or double door
    public bool twoDoors = false;
    // Whether or not it rotates (as opposed to slides)
    public bool rotate = true;
    // Speed at which it will move
    public float openSpeed = 0.1F;
    // If the door rotates, how many degrees it what directions it will rotate
    public Vector3 rotation;

    // Left & right doors
    public GameObject leftDoor;
    public GameObject rightDoor;

    // Variables for whether or not to open or close the doors
    private bool openDoors = false;
    private bool closeDoors = false;
    // Whether or not the door is currently open
    private bool open = false;

    // Transform components of the doors
    private Transform leftDoorTransform;
    private Transform rightDoorTransform;

    // Starting rotations of rotating doors
    private Quaternion leftDoorStartRotation;
    private Quaternion rightDoorStartRotation;
    // Starting positions of sliding doors
    private Vector3 leftDoorStartPosition;
    private Vector3 rightDoorStartPosition;

    // Ending rotations of rotating doors
    private Quaternion leftDoorEndRotation;
    private Quaternion rightDoorEndRotation;
    // Ending positions of sliding doors
    private Vector3 leftDoorEndPosition;
    private Vector3 rightDoorEndPosition;

    // Use this for initialization
    void Start ()
    {
        // Get the transform component for the left door
        leftDoorTransform = leftDoor.GetComponent<Transform>();

        if (rotate)
        {
            // If it's a rotating door, get the rotation and then find the end rotation by multiplying it times the intended rotation
            leftDoorStartRotation = leftDoorTransform.rotation;
            leftDoorEndRotation = leftDoorStartRotation * Quaternion.Euler(rotation);
        }
        else
        {
            // If it's a sliding door, get the position and then find the end position by adding 2 times the x values of the door
            leftDoorStartPosition = leftDoorTransform.position;
            leftDoorEndPosition = leftDoorStartPosition + new Vector3(leftDoorStartPosition.x * 2, 0, 0);
        }

        // If there are two doors, do it all again for the right door
        if (twoDoors)
        {
            // Get the transform component for the right door
            rightDoorTransform = rightDoor.GetComponent<Transform>();

            if (rotate)
            {
                // If it's a rotating door, get the rotation and then find the end rotation by multiplying it times the intended rotation
                rightDoorStartRotation = rightDoorTransform.rotation;
                rightDoorEndRotation = rightDoorStartRotation * Quaternion.Euler(rotation);
            }
            else
            {
                // If it's a sliding door, get the position and then find the end position by adding 2 times the x values of the door
                rightDoorStartPosition = rightDoorTransform.position;
                rightDoorEndPosition = rightDoorStartPosition + new Vector3(rightDoorStartPosition.x * 2, 0, 0);
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Do the move doors function (checks if it should be opening or closing)
        MoveDoors();
	}

    // Function to set the doors to start opening
    public void Open()
    {
        closeDoors = false;
        openDoors = true;
    }

    // Function to set the doors to start closing
    public void Close()
    {
        openDoors = false;
        closeDoors = true;
    }

    // Function to open the doors instantly (used for loading save state)
    public void OpenImmediately()
    {
        // Set the variables that tell the doors to move to false
        openDoors = closeDoors = false;

        if (rotate)
        {
            // If it's a rotating door, set the rotation to the end rotation
            leftDoorTransform.rotation = leftDoorEndRotation;

            // If there are 2 doors, do the same for the right door
            if (twoDoors)
            {
                rightDoorTransform.rotation = rightDoorEndRotation;
            }
        }
        else
        {
            // If it's a sliding door, set the position to the end position
            leftDoorTransform.position = leftDoorEndPosition;

            // If there are 2 doors, do the same for the right door
            if (twoDoors)
            {
                rightDoorTransform.position = rightDoorEndPosition;
            }
        }
        
        // Set open to true since the door is now open
        open = true;
    }

    // Function to close the doors instantly (used for loading save state)
    public void CloseImmediately()
    {
        // Set the variables that tell the doors to move to false
        openDoors = closeDoors = false;

        if (rotate)
        {
            // If it's a rotating door, set the rotation to the start rotation
            leftDoorTransform.rotation = leftDoorStartRotation;

            // If there are 2 doors, do the same for the right door
            if (twoDoors)
            {
                rightDoorTransform.rotation = rightDoorStartRotation;
            }
        }
        else
        {
            // If it's a sliding door, set the position to the end position
            leftDoorTransform.position = leftDoorStartPosition;

            // If there are 2 doors, do the same for the right door
            if (twoDoors)
            {
                rightDoorTransform.position = rightDoorStartPosition;
            }
        }

        // Set open to false since the door is closed now
        open = false;
    }

    // Return whether or not the door is open
    public bool isOpen()
    {
        return open;
    }

    private void MoveDoors()
    {
        // Stuff to do when opening the doors
        if(openDoors)
        {
            if(rotate)
            {
                // If rotating doors, change the rotation of the door and rotate towards the final rotation at the intended speed
                leftDoorTransform.rotation = Quaternion.RotateTowards(leftDoorTransform.rotation, leftDoorEndRotation, openSpeed);
                // If there are two doors, do the same for the right
                if(twoDoors)
                {
                    rightDoorTransform.rotation = Quaternion.RotateTowards(rightDoorTransform.rotation, rightDoorEndRotation, openSpeed);
                }

                // Check to see if the doors are fully open. If they are, stop doing calculations
                // Don't ask where this logic came from. I just kind of spit it out and it worked and I'm not questioning it
                if (leftDoorTransform.rotation == leftDoorEndRotation && (!twoDoors || rightDoorTransform.rotation == rightDoorEndRotation))
                {
                    openDoors = false;
                }
            }
            else
            {
                // If sliding doors, move towards the ending position at the intended speed
                leftDoorTransform.position = Vector3.MoveTowards(leftDoorTransform.position, leftDoorEndPosition, openSpeed);
                // If there are two doors, do the same for the right
                if(twoDoors)
                {
                    rightDoorTransform.position = Vector3.MoveTowards(rightDoorTransform.position, rightDoorEndPosition, openSpeed);
                }

                // Check to see if the doors are fully closed. If they are, stop doing calculations
                // Don't ask where this logic came from. I just kind of spit it out and it worked and I'm not questioning it
                // I think it's obvious that I'm copying and pasting comments right now. It all does the same thing, right?
                if (leftDoorTransform.position == leftDoorEndPosition && (!twoDoors || rightDoorTransform.position == rightDoorEndPosition))
                {
                    openDoors = false;
                }
            }

            // Set the open bool to true (the door is now open) (it does this even when slightly open)
            open = true;
        }
        // Stuff to do when closing the doors
        else if (closeDoors)
        {
            if (rotate)
            {
                // If rotating doors, change the rotation of the door and rotate towards the starting rotation at the intended speed
                leftDoorTransform.rotation = Quaternion.RotateTowards(leftDoorTransform.rotation, leftDoorStartRotation, openSpeed);
                // If there are two doors, do the same for the right
                if (twoDoors)
                {
                    rightDoorTransform.rotation = Quaternion.RotateTowards(rightDoorTransform.rotation, rightDoorStartRotation, openSpeed);
                }

                // Check to see if the doors are fully closed. If they are, stop doing calculations and set the open bool to false (it does this only when fully closed)
                // Don't ask where this logic came from. I just kind of spit it out and it worked and I'm not questioning it
                // I think it's obvious that I'm copying and pasting comments right now. It all does the same thing, right?
                if (leftDoorTransform.rotation == leftDoorStartRotation && (!twoDoors || rightDoorTransform.rotation == rightDoorStartRotation))
                {
                    closeDoors = false;
                    open = false;
                }
            }
            else
            {
                // If sliding doors, move towards the starting position at the intended speed
                leftDoorTransform.position = Vector3.MoveTowards(leftDoorTransform.position, leftDoorStartPosition, openSpeed);
                // If there are two doors, do the same for the right
                if (twoDoors)
                {
                    rightDoorTransform.position = Vector3.MoveTowards(rightDoorTransform.position, rightDoorStartPosition, openSpeed);
                }

                // Check to see if the doors are fully closed. If they are, stop doing calculations and set the open bool to false (it does this only when fully closed)
                // Don't ask where this logic came from. I just kind of spit it out and it worked and I'm not questioning it
                // I think it's obvious that I'm copying and pasting comments right now. It all does the same thing, right?
                if (leftDoorTransform.position == leftDoorStartPosition && (!twoDoors || rightDoorTransform.position == rightDoorStartPosition))
                {
                    closeDoors = false;
                    open = false;
                }
            }
        }
        else
        {
            // This should never happen but whatever. Just in case, stop moving in both directions so it doesn't calculate things over and over as it is stuck
            openDoors = closeDoors = false;
        }
    }
}
